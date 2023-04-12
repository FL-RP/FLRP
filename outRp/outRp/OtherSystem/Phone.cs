

/*

namespace outRp.OtherSystem
{
public class Phones 
{

#region Phone Model
public class PhoneModel
{
    public string backgroundImage { get; set; }
    public int phoneNumber { get; set; }
    public List<Contacts> contacts { get; set; } = new List<Contacts>();
    public List<MessageList> messages { get; set; } = new List<MessageList>();
}

public class Contacts
{
    public string ID { get; set; }
    public int PhoneNumber { get; set; }
    public string ProfilePhoto { get; set; }
}

public class MessageList
{
    public int PhoneNumber { get; set; }
    public List<MessageModel> Messages { get; set; } = new List<MessageModel>();
}
public class MessageModel
{
    public string Text { get; set; }
    public Position GPS { get; set; }
    public DateTime Date { get; set; }
    public bool isOwner { get; set; }
}

#endregion


public class PhoneConst
{
    //SET - GET DATA
    public const string PlayerPhoneData = "PlayerPhoneData";

    public const string PlayerOnPhoneTo = "PlayerOnPhoneTo";
    public const string PlayerPhoneContactName = "PlayerPhoneContactName";
    public const string PlayerPhoneID = "PlayerPhoneID";
}

public static PhoneModel getPhone(PlayerModel p)
{
    if (p.HasSyncedMetaData(PhoneConst.PlayerPhoneData))
    {
        p.GetSyncedMetaData<string>(PhoneConst.PlayerPhoneData, out string x);
        PhoneModel a = JsonConvert.DeserializeObject<PhoneModel>(x);
        return a;
    }
    else
    {
        return null;
    }

}
public static void savePhone(PlayerModel p, PhoneModel x)
{
    string json = JsonConvert.SerializeObject(x);
    p.SetSyncedMetaData(PhoneConst.PlayerPhoneData, json);
    p.Emit("Phone:Update");
    return;
}

public static void TakePhone(PlayerModel p, InventoryModel i)
{
    if (p.HasData(PhoneConst.PlayerPhoneID))
    {
        p.GetData<int>(PhoneConst.PlayerPhoneID, out int playerPhoneID);
        InventoryModel takedPhone = Database.DatabaseMain.FindInventoryItem(playerPhoneID);
        p.GetSyncedMetaData<string>(PhoneConst.PlayerPhoneData, out string pPhoneData);
        takedPhone.itemData2 = pPhoneData;
        takedPhone.itemSlot = 0;
        takedPhone.Update();
    }

    int number = Int32.Parse(i.itemData);
    p.phoneNumber = number;
    string phoneData = i.itemData2;
    i.itemSlot = 12;
    i.Update();
    p.SetData(PhoneConst.PlayerPhoneID, i.ID);
    p.SetSyncedMetaData(PhoneConst.PlayerPhoneData, phoneData);
    p.Emit("Phone:Setup");
    p.SendChatMessage("> Telefon etkileşimi için F3 tuşunu kullanın.");
    Inventory.UpdatePlayerInventory(p);
}
public static void TakeOffPhone(PlayerModel p, InventoryModel i)
{
    try
    {
        p.GetData<int>(PhoneConst.PlayerPhoneID, out int playerPhoneID);
        p.GetSyncedMetaData<string>(PhoneConst.PlayerPhoneData, out string pPhoneData);
        i.itemData2 = pPhoneData;
        i.itemSlot = 0;
        i.Update();
        p.phoneNumber = 0;
        p.DeleteData(PhoneConst.PlayerPhoneID);
        p.DeleteSyncedMetaData(PhoneConst.PlayerPhoneData);
        Inventory.UpdatePlayerInventory(p);
        p.Emit("Phone:Destroy");
    }
    catch { }

}

#region Listeners
[AsyncClientEvent("Phone:OnHand")]
public static void PlayerPhoneTakeHand(PlayerModel p, bool status)
{
    if (status)
    {
        if (!p.HasData(PhoneConst.PlayerOnPhoneTo))
        {
            if(p.Vehicle == null)
            {
                Animations.PlayerAnimation(p, "phone2");
            }

        }
    }
    else
    {
        if (!p.HasData(PhoneConst.PlayerOnPhoneTo))
        {
            if (p.Vehicle == null)
            {
                Animations.PlayerStopAnimation(p);
            }
        }

    }
}

[AsyncClientEvent("Phone:Call")]
public static void Phone_Call(PlayerModel p, string n)
{
    int number = Int32.Parse(n);
    if(p.phoneNumber == 0) { p.SendChatMessage("Telefonunuz yok."); return; }
    if(p.phoneNumber == number) { p.SendChatMessage("Kendiniz arayamazsınız."); return; }
    if(number == 911)
    {
        p.SendChatMessage(" [911] İhbar Hattı: Hat üzerinde görüşmek istediğiniz acil birimi belirtiniz? <br> Polis | Hastane ");
        p.SetData(EntityData.PlayerEntityData.PlayerInEmergencyCall_1, 0);
        return;
    }
    PlayerModel target = null;
    foreach(PlayerModel t in Alt.GetAllPlayers())
    {
        if(t.phoneNumber == number)
        {
            target = t;
            break;
        }
    }
    if(target == null) { p.Emit("Phone:TargetBusy"); return; }
    if (target.HasData(PhoneConst.PlayerOnPhoneTo)) { p.Emit("Phone:TargetBusy"); return; }

    Animations.PlayerAnimation(p, "phone1");

    target.Emit("Phone:IncomingCall", p.phoneNumber);
    GlobalEvents.NativeNotifyAll(target, "~p~Telefonu çalıyor.");
}
[AsyncClientEvent("Phone:AcceptCall")]
public static void Phone_AcceptCall(PlayerModel p, int acceptedNumber)
{
    PlayerModel target = null;
    foreach(PlayerModel t in Alt.GetAllPlayers())
    {
        if(t.phoneNumber == acceptedNumber)
        {
            target = t;
            break;
        }
    }

    MainChat.SendInfoChat(p, "{FF9832}> Telefonu açtınız.");
    MainChat.SendInfoChat(target, "{FF9832}> Karşı taraf telefonu açtı.");

    p.SetData(PhoneConst.PlayerOnPhoneTo, target.sqlID);
    PhoneModel playerPhone = getPhone(p);
    var targetContactName = playerPhone.contacts.Find(x => x.PhoneNumber == acceptedNumber);
    if (targetContactName != null) { p.SetData(PhoneConst.PlayerPhoneContactName, targetContactName.ID); }
    else { p.SetData(PhoneConst.PlayerPhoneContactName, acceptedNumber.ToString()); }

    target.SetData(PhoneConst.PlayerOnPhoneTo, p.sqlID);
    PhoneModel targetPhone = getPhone(p);
    var playerContactName = targetPhone.contacts.Find(x => x.PhoneNumber == p.phoneNumber);
    if (playerContactName != null) { target.SetData(PhoneConst.PlayerPhoneContactName, playerContactName.ID); }
    else { target.SetData(PhoneConst.PlayerPhoneContactName, p.phoneNumber.ToString()); }

    Animations.PlayerAnimation(p, "phone1");
    Animations.PlayerAnimation(target, "phone1");

    p.Emit("Phone:JoinCall");
    target.Emit("Phone:JoinCall");
}
[AsyncClientEvent("Phone:DeclineCall")]
public static void Phone_DeclineCall(PlayerModel p, int declinedNumber)
{
    PlayerModel target = null;
    foreach(PlayerModel t in Alt.GetAllPlayers())
    {
        if(t.phoneNumber == declinedNumber)
        {
            target = t;
        }
    }
    if(target == null) { return; }
    target.Emit("Phone:TargetBusy");
}
[AsyncClientEvent("Phone:CloseActiveCall")]
public static void Phone_CloseActiveCall(PlayerModel p)
{
    PlayerModel target = null;
    if (p.HasData(PhoneConst.PlayerPhoneContactName))
    {
        int tSql = p.lscGetdata<int>(PhoneConst.PlayerOnPhoneTo);
        target = GlobalEvents.GetPlayerFromSqlID(tSql);
        p.DeleteData(PhoneConst.PlayerOnPhoneTo);
        p.DeleteData(PhoneConst.PlayerPhoneContactName);
    }
    if(target == null) { return; }

    if (target.HasData(PhoneConst.PlayerPhoneContactName)) { target.DeleteData(PhoneConst.PlayerPhoneContactName); }
    if (target.HasData(PhoneConst.PlayerOnPhoneTo)) { target.DeleteData(PhoneConst.PlayerOnPhoneTo); }
    target.Emit("Phone:CloseCurrentCall");
    MainChat.SendInfoChat(p, "Telefonu kapattınız.");
    MainChat.SendInfoChat(target, "Karşı taraf telefonu kapattı.");

    if(p.Vehicle == null) { Animations.PlayerStopAnimation(p); }
    if(target.Vehicle == null) { Animations.PlayerStopAnimation(target); }            
}

[AsyncClientEvent("Phone:SendSMS")]
public static void Phone_SendSMS(PlayerModel p, string num, string message)
{           
    int number = Int32.Parse(num);
    PlayerModel target = null;
    foreach (PlayerModel t in Alt.GetAllPlayers())
    {
        if(t.phoneNumber == number)
        {
            target = t;
            break;
        }
    }
    if(target == null) { p.SendChatMessage("Numara bulunamadı!"); return; }//Kişiye telefon bildirimi gönder.

    MessageModel newMessage = new MessageModel();
    newMessage.Date = DateTime.Now;
    newMessage.Text = message;
    newMessage.isOwner = false;


    PhoneModel tPhone = getPhone(target);
    var x = tPhone.messages.Find(x => x.PhoneNumber == p.phoneNumber);
    if(x != null)
    {
        x.Messages.Add(newMessage);
    }
    else
    {
        x = new MessageList();
        x.PhoneNumber = p.phoneNumber;
        x.Messages.Add(newMessage);
        tPhone.messages.Add(x);
    }
    savePhone(target, tPhone);


    newMessage.isOwner = true;
    PhoneModel pPhone = getPhone(p);
    var y = pPhone.messages.Find(x => x.PhoneNumber == target.phoneNumber);
    if (y != null)
    {
        y.Messages.Add(newMessage);
    }
    else
    {
        y = new MessageList();
        y.PhoneNumber = target.phoneNumber;
        y.Messages.Add(newMessage);
        pPhone.messages.Add(y);
    }
    savePhone(p, pPhone);

    string name = "n";
    var playercontactName = tPhone.contacts.Find(x => x.PhoneNumber == p.phoneNumber);
    if (playercontactName != null) { name = playercontactName.ID; }
    else { name = number.ToString(); }

    GlobalEvents.NativeNotify(target, "~y~Yeni Mesaj Geldi~n~~w~" + name);

}
[AsyncClientEvent("Phone:SendGPS")]
public static void Phone_SendGPS(PlayerModel p, int number, Position gps)
{
    PlayerModel target = null;
    foreach(PlayerModel t in Alt.GetAllPlayers())
    {
        if(t.phoneNumber == number)
        {
            target = t;
        }
    }
    if(target == null) { p.SendChatMessage("Numara bulunamadı!"); return; }//Kişiye telefon bildirimi gönder.
}

// Rehber 
[AsyncClientEvent("Phone:AddNewContactToList")]
public static void Phone_AddNewContact(PlayerModel p, string name, string num)
{
    int number = Int32.Parse(num);
    PhoneModel pP = getPhone(p);
    Contacts newC = new Contacts();
    newC.ID = name;
    newC.PhoneNumber = number;
    pP.contacts.Add(newC);
    savePhone(p, pP);

}

[AsyncClientEvent("Phone:DeleteContactToList")]
public static void Phone_RemoveContact(PlayerModel p, int number)
{
    PhoneModel pP = getPhone(p);
    var x = pP.contacts.Find(x => x.PhoneNumber == number);
    if(x == null) { return; }
    pP.contacts.Remove(x);
    savePhone(p, pP);
}

[AsyncClientEvent("Phone:DeleteMessage")]
public static void Phone_DeleteMessage(PlayerModel p, int number)
{
    PhoneModel pP = getPhone(p);
    var x = pP.messages.Find(x => x.PhoneNumber == number);
    if(x == null) { return; }
    pP.messages.Remove(x);
    savePhone(p, pP);
}
#endregion

}
}


/*
InventoryModel x = playerInventory.Find(x => x.itemId == 1 && x.itemSlot == 12);
    if(x == null)
    {
        if (player.HasData(Phone.PhoneConst.PlayerPhoneData))
        {
            if (player.HasData(EntityData.PlayerEntityData.PlayerPhoneId))
            {
                player.GetData<int>(EntityData.PlayerEntityData.PlayerPhoneId, out int PlayerPhoneID);
                InventoryModel phone = DatabaseMain.FindInventoryItem(PlayerPhoneID);
                player.GetData<string>(Phone.PhoneConst.PlayerPhoneData, out string PlayerPhoneData);
                phone.itemData2 = PlayerPhoneData;
                phone.Update();
                player.phoneNumber = 0;
                player.DeleteData(Phone.PhoneConst.PlayerPhoneData);
                player.DeleteData(EntityData.PlayerEntityData.PlayerPhoneId);
            }
        }
    }
    else
    {
        if (!player.HasData(Phone.PhoneConst.PlayerPhoneData))
        {
            if (!player.HasData(EntityData.PlayerEntityData.PlayerPhoneId))
            {
                player.phoneNumber = Int32.Parse(x.itemData);
                player.SetData(Phone.PhoneConst.PlayerPhoneData, x.itemData2);
                Phone.TakePhone(player, x.itemData2);
            }
        }
    }
*/