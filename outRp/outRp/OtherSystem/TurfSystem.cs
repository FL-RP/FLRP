using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Numerics;
using AltV.Net;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;

namespace outRp.OtherSystem
{
    public class TurfSystem : IScript
    {
        public TurfSystem()
        {
            Alt.Log("加载 地盘系统.");
        }

        public class TurfModel : TurfSaveModel
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; } = "";
            public List<Position> positions { get; set; }
            public bool isLegal { get; set; } = true;
            [NotMapped]
            public IColShape shape { get; set; } = null;
        }

        public interface TurfSaveModel 
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public List<Position> positions { get; set; }
            public bool isLegal { get; set; }
        }

        private static List<TurfModel> _turfs = new();
        public static List<TurfModel> turfs
        {
            get { return _turfs; }
            set
            {
                _turfs = value;
            }
        }
        


        public static TurfModel GetTurf(int id)
        {
            return turfs.Where(x => x.Id == id).FirstOrDefault();
        }
        public static TurfModel GetTurf(IColShape shape)
        {
            return turfs.Where(x => x.shape == shape).FirstOrDefault();
        }
        
        public static List<TurfModel> SaveTurfVoid()
        {
            turfs.ForEach(x =>
            {
                x.shape = null;
            });

            return turfs;
        }
        public static void LoadAllTurfs(List<TurfModel> tt)
        {
            turfs = tt;
            turfs.ForEach(x =>
            {
                ReplaceTurf(x);
            });
            Alt.Log("地盘系统加载完成, 总计: " + turfs.Count);
        }

        public static (float, float, Vector2[]) CalculatePosition(List<Position> _pos)
        {
            List<Vector2> pos = new();
            float minZ = 0;
            float maxZ = 0;
            foreach(Position p in _pos)
            {
                if (p.Z < minZ)
                    minZ = p.Z;

                if (p.Z > maxZ)
                    maxZ = p.Z;

                pos.Add(new() { X = p.X, Y = p.Y });
            }

            return (minZ, maxZ, pos.ToArray());
        }

        public static void ReplaceTurf(TurfModel turf)
        {
            if (turf.positions.Count < 3)
                return;

            var info = CalculatePosition(turf.positions);
            if(turf.shape == null)
            {
                turf.shape = Alt.CreateColShapePolygon(info.Item1 - 5, info.Item2 + 5, info.Item3);
            }
            else
            {
                turf.shape.Remove();
                turf.shape = Alt.CreateColShapePolygon(info.Item1 - 5, info.Item2 + 5, info.Item3);
            }
                
        }

        /*        [Command("turfolustur")]
                public void CreateTurf(PlayerModel player, params string[] args)
                {
                    if(player.adminLevel < 5) { MainChat.SendErrorChat(player, "[HATA] Bunun için yetkisi yok."); return; }
                    if(args.Length <= 0) { MainChat.SendInfoChat(player, "[Kullanım] /turfolustur [isim]"); return; }
                    TurfModel n = new();
                    n.Name = string.Join(" ", args);
                    n.positions = new();

                    if (turfs.Count == 0)
                        n.Id = 1;
                    else
                    {
                        n.Id = turfs.Last().Id + 1;
                    }

                    turfs.Add(n);
                    MainChat.SendInfoChat(player, "[?] Turf başarıyla oluşturuldu. G.t.m K.ll. ID: " + n.Id);
                    return;
                }

                [Command("tpozisyonekle")]
                public void AddPositionToTurf(PlayerModel p, params string[] args)
                {
                    if (p.adminLevel < 5) { MainChat.SendErrorChat(p, "[HATA] Bunun için yetkisi yok."); return; }
                    if (args.Length <= 0)
                    {
                        MainChat.SendInfoChat(p, "[Kullanım] /tpozisyonekle [id]");
                        return;
                    }

                    if(!Int32.TryParse(args[0], out int tid)) { MainChat.SendInfoChat(p, "[Kullanım] /tpozisyonekle [id]"); return; }

                    var turf = GetTurf(tid);
                    if(turf == null) { MainChat.SendErrorChat(p, "[HATA] Turf bölgesi bulunamadı!"); return; }

                    turf.positions.Add(p.Position);            
                    ReplaceTurf(turf);
                    MainChat.SendInfoChat(p, "[?] Turf için pozisyon eklendi.");
                    return;
                }

                [Command("turfduzenle")]
                public void EditTurf(PlayerModel p, params string[] args)
                {
                    if (p.adminLevel < 5) { MainChat.SendErrorChat(p, "[HATA] Bunun için yetkisi yok."); return; }
                    if(args.Length <= 1) { MainChat.SendInfoChat(p, "[Kullanım] /turfduzenle [id] [cesit] [varsa değer]<br> Kullanılabilecek değerler: isim-aciklama-illegal"); return; }
                    if(!Int32.TryParse(args[0], out int id)) { MainChat.SendInfoChat(p, "[Kullanım] /turfduzenle [id] [cesit] [varsa değer]<br> Kullanılabilecek değerler: isim-aciklama-illegal"); return; }

                    var turf = GetTurf(id);
                    if(turf == null) { MainChat.SendErrorChat(p, "[HATA] Turf bulunamadı."); return; }

                    switch(args[1])
                    {
                        case "isim":
                            turf.Name = String.Join(" ", args[2..]);
                            MainChat.SendInfoChat(p, "[?] Turf adı değiştirildi.");
                            return;

                        case "aciklama":
                            turf.Description = String.Join(" ", args[2..]);
                            MainChat.SendInfoChat(p, "[?] Turf açıklaması değiştirildi.");
                            break;

                        case "illegal":
                            turf.isLegal = !turf.isLegal;
                            MainChat.SendInfoChat(p, "[?] Turf durumu " + ((turf.isLegal) ? "legal" : "illegal") + " olarak ayarlandı.");
                            return;
                    }
                }

                [Command("turfkaldir")]
                public void RemoveTurf(PlayerModel p, params string[] args)
                {
                    if (p.adminLevel < 5) { MainChat.SendErrorChat(p, "[HATA] Bunun için yetkisi yok."); return; }
                    if (args.Length <= 0) { MainChat.SendInfoChat(p, "[Kullanım] /turfkaldir [id]"); return; }
                    if (!Int32.TryParse(args[0], out int tid)) { MainChat.SendInfoChat(p, "[Kullanım] /turfkaldir [id]"); return; }
                    var turf = GetTurf(tid);
                    if(turf == null) { MainChat.SendErrorChat(p, "[HATA] Turf bulunamadı!"); return; }
                    turfs.Remove(turf);
                    MainChat.SendInfoChat(p, "[?] Turf başarıyla kaldırıldı.");
                    return;
                }

                [Command("turfliste")]
                public void ShowTurfList(PlayerModel p)
                {
                    if (p.adminLevel < 5) { MainChat.SendErrorChat(p, "[HATA] Bunun için yetkisi yok."); return; }
                    MainChat.SendInfoChat(p, "<center>Turf Listesi</center>");
                    foreach(var turf in turfs)
                    {
                        MainChat.SendInfoChat(p, "[ID: " + turf.Id + "] " + turf.Name + ((turf.Description.Length > 0) ? "<br>" + turf.Description : ""));
                    }
                    return;
                }

                [ScriptEvent(ScriptEventType.ColShape)]
                public async void OnColshape(IColShape _shape, IEntity entity, bool state)
                {
                    if (entity is not IPlayer)
                        return;

                    PlayerModel target = entity as PlayerModel;

                    var sh = GetTurf(_shape);
                    if (sh == null)
                        return;

                    if(state)
                    {
                        if (target.Dimension != 0)
                            return;
                        if(!sh.isLegal)
                        {
                            var fact = await Database.DatabaseMain.GetFactionInfo(target.factionId);
                            if(fact.type == 2 ||fact.type == 7)
                                target.EmitLocked("Turf:SetName", sh.Name);
                        }
                        else
                        {
                            target.EmitLocked("Turf:SetName", sh.Name);
                        }
                    }
                    else
                    {
                        target.EmitLocked("Turf:SetName", "");
                    }
                }*/
    }
}
