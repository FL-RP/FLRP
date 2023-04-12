using AltV.Net; using AltV.Net.Async;
using outRp.Chat;
using outRp.Globals;
using outRp.Models;
using System.Collections.Generic;
using System.Linq;
using AltV.Net.Resources.Chat.Api;

namespace outRp.OtherSystem
{
    public class Animations : IScript
    {
        public class AnimModel
        {
            //public string name { get; set; }
            public string dict { get; set; }
            public string anim { get; set; }
            public string displayName { get; set; }
            public AnimOptions AnimOptions { get; set; } = null;

            public AnimModel(string Dict, string Anim, string dName, AnimOptions AnimOptions = null)
            {
                this.dict = Dict;
                this.anim = Anim;
                this.displayName = dName;
                this.AnimOptions = AnimOptions ?? null;
            }
        }
        public class AnimOptions
        {
            public string Prop { get; set; } = null;
            public int? PropBone { get; set; } = null;
            public object[] PropPlacement { get; set; } = null;
            public string SecondProp { get; set; } = null;
            public int? SecondPropBone { get; set; } = null;
            public object[] SecondPropPlacement { get; set; } = null;
            public bool? EmoteLoop { get; set; } = null;
            public bool? EmoteMoving { get; set; } = null;
            public int? EmoteDuration { get; set; } = null;
        }

        public static Dictionary<string, AnimModel> Anims = new Dictionary<string, AnimModel>()
        {
            ["dance1"] = new AnimModel(new string("anim@amb@nightclub@dancers@solomun_entourage@"), new string("mi_dance_facedj_17_v1_female^1"), new string("舞蹈 F"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["dance2"] = new AnimModel(new string("anim@amb@nightclub@mini@dance@dance_solo@female@var_a@"), new string("high_center"), new string("舞蹈 F2"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["dance3"] = new AnimModel(new string("anim@amb@nightclub@mini@dance@dance_solo@female@var_a@"), new string("high_center_up"), new string("舞蹈 F3"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["dance4"] = new AnimModel(new string("anim@amb@nightclub@dancers@crowddance_facedj@hi_intensity"), new string("hi_dance_facedj_09_v2_female^1"), new string("舞蹈 F4"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["dance5"] = new AnimModel(new string("anim@amb@nightclub@dancers@crowddance_facedj@hi_intensity"), new string("hi_dance_facedj_09_v2_female^3"), new string("舞蹈 F5"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["dance6"] = new AnimModel(new string("anim@amb@nightclub@mini@dance@dance_solo@female@var_a@"), new string("high_center_up"), new string("舞蹈 F6"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["dance7"] = new AnimModel(new string("anim@amb@nightclub@mini@dance@dance_solo@female@var_a@"), new string("low_center"), new string("慢舞 2"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["dance8"] = new AnimModel(new string("anim@amb@nightclub@mini@dance@dance_solo@female@var_a@"), new string("low_center_down"), new string("慢舞 3"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["dance9"] = new AnimModel(new string("anim@amb@nightclub@mini@dance@dance_solo@female@var_b@"), new string("low_center"), new string("慢舞 4"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["dance10"] = new AnimModel(new string("anim@amb@nightclub@dancers@podium_dancers@"), new string("hi_dance_facedj_17_v2_male^5"), new string("舞蹈"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["dance11"] = new AnimModel(new string("anim@amb@nightclub@mini@dance@dance_solo@male@var_b@"), new string("high_center_down"), new string("舞蹈 2"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["dance12"] = new AnimModel(new string("anim@amb@nightclub@mini@dance@dance_solo@male@var_a@"), new string("high_center"), new string("舞蹈 3"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["dance13"] = new AnimModel(new string("anim@amb@nightclub@mini@dance@dance_solo@male@var_b@"), new string("high_center_up"), new string("舞蹈 4"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["dance14"] = new AnimModel(new string("anim@amb@nightclub@mini@dance@dance_solo@female@var_b@"), new string("high_center"), new string("舞蹈 上升"), new AnimOptions
            {
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["dance15"] = new AnimModel(new string("anim@amb@nightclub@mini@dance@dance_solo@female@var_b@"), new string("high_center_up"), new string("舞蹈 上升 2"), new AnimOptions
            {
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["dance16"] = new AnimModel(new string("anim@amb@nightclub@mini@dance@dance_solo@male@var_a@"), new string("low_center"), new string("舞蹈 害羞"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["dance17"] = new AnimModel(new string("anim@amb@nightclub@mini@dance@dance_solo@female@var_b@"), new string("low_center_down"), new string("舞蹈 害羞 2"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["dance18"] = new AnimModel(new string("anim@amb@nightclub@mini@dance@dance_solo@male@var_b@"), new string("low_center"), new string("慢舞"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["dance19"] = new AnimModel(new string("rcmnigel1bnmt_1b"), new string("dance_loop_tyler"), new string("愚钝舞蹈 9"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["dance20"] = new AnimModel(new string("misschinese2_crystalmazemcs1_cs"), new string("dance_loop_tao"), new string("舞蹈 6"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["dance21"] = new AnimModel(new string("misschinese2_crystalmazemcs1_ig"), new string("dance_loop_tao"), new string("舞蹈 7"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["dance22"] = new AnimModel(new string("missfbi3_sniping"), new string("dance_m_default"), new string("舞蹈 8"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["dance23"] = new AnimModel(new string("special_ped@mountain_dancer@monologue_3@monologue_3a"), new string("mnt_dnc_buttwag"), new string("愚钝舞蹈"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["dance24"] = new AnimModel(new string("move_clown@p_m_zero_idles@"), new string("fidget_short_dance"), new string("愚钝舞蹈 2"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["dance25"] = new AnimModel(new string("move_clown@p_m_two_idles@"), new string("fidget_short_dance"), new string("愚钝舞蹈 3"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["dance26"] = new AnimModel(new string("anim@amb@nightclub@lazlow@hi_podium@"), new string("danceidle_hi_11_buttwiggle_b_laz"), new string("愚钝舞蹈 4"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["dance27"] = new AnimModel(new string("timetable@tracy@ig_5@idle_a"), new string("idle_a"), new string("愚钝舞蹈 5"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["dance28"] = new AnimModel(new string("timetable@tracy@ig_8@idle_b"), new string("idle_d"), new string("愚钝舞蹈 6"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["dance28"] = new AnimModel(new string("anim@amb@nightclub@mini@dance@dance_solo@female@var_a@"), new string("med_center_up"), new string("舞蹈 9"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["dance29"] = new AnimModel(new string("anim@mp_player_intcelebrationfemale@the_woogie"), new string("the_woogie"), new string("愚钝舞蹈 8"), new AnimOptions
            {
                EmoteLoop = true
            }),
            ["dance30"] = new AnimModel(new string("anim@amb@casino@mini@dance@dance_solo@female@var_b@"), new string("high_center"), new string("愚钝舞蹈 7"), new AnimOptions
            {
                EmoteLoop = true
            }),
            ["dance31"] = new AnimModel(new string("anim@amb@casino@mini@dance@dance_solo@female@var_a@"), new string("med_center"), new string("舞蹈 5"), new AnimOptions
            {
                EmoteLoop = true
            }),
            ["dancestick1"] = new AnimModel(new string("anim@amb@nightclub@lazlow@hi_railing@"), new string("ambclub_13_mi_hi_sexualgriding_laz"), new string("舞蹈 荧光棒"), new AnimOptions
            {
                Prop = "ba_prop_battle_glowstick_01",
                PropBone = 28422,
                PropPlacement = new object[] { 0.0700, 0.1400, 0.0, -80.0, 20.0, 0 },
                SecondProp = "ba_prop_battle_glowstick_01",
                SecondPropBone = 60309,
                SecondPropPlacement = new object[] { 0.0700, 0.0900, 0.0, -120.0, -20.0, 0 },
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["dancestick2"] = new AnimModel(new string("anim@amb@nightclub@lazlow@hi_railing@"), new string("ambclub_12_mi_hi_bootyshake_laz"), new string("舞蹈 荧光棒 2"), new AnimOptions
            {
                Prop = "ba_prop_battle_glowstick_01",
                PropBone = 28422,
                PropPlacement = new object[] { 0.0700, 0.1400, 0.0, -80.0, 20.0, 0 },
                SecondProp = "ba_prop_battle_glowstick_01",
                SecondPropBone = 60309,
                SecondPropPlacement = new object[] { 0.0700, 0.0900, 0.0, -120.0, -20.0, 0 },
                EmoteLoop = true,
            }),
            ["dancestick2"] = new AnimModel(new string("anim@amb@nightclub@lazlow@hi_railing@"), new string("ambclub_09_mi_hi_bellydancer_laz"), new string("舞蹈 荧光棒 3"), new AnimOptions
            {
                Prop = "ba_prop_battle_glowstick_01",
                PropBone = 28422,
                PropPlacement = new object[] { 0.0700, 0.1400, 0.0, -80.0, 20.0, 0 },
                SecondProp = "ba_prop_battle_glowstick_01",
                SecondPropBone = 60309,
                SecondPropPlacement = new object[] { 0.0700, 0.0900, 0.0, -120.0, -20.0, 0 },
                EmoteLoop = true,
            }),
            ["dancehorse1"] = new AnimModel(new string("anim@amb@nightclub@lazlow@hi_dancefloor@"), new string("dancecrowd_li_15_handup_laz"), new string("舞蹈 马"), new AnimOptions
            {
                Prop = "ba_prop_battle_hobby_horse",
                PropBone = 28422,
                PropPlacement = new object[] { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 },
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["dancehorse2"] = new AnimModel(new string("anim@amb@nightclub@lazlow@hi_dancefloor@"), new string("crowddance_hi_11_handup_laz"), new string("舞蹈 马 2"), new AnimOptions
            {
                Prop = "ba_prop_battle_hobby_horse",
                PropBone = 28422,
                PropPlacement = new object[] { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 },
                EmoteLoop = true,
            }),
            ["dancehorse3"] = new AnimModel(new string("anim@amb@nightclub@lazlow@hi_dancefloor@"), new string("dancecrowd_li_11_hu_shimmy_laz"), new string("舞蹈 马 3"), new AnimOptions
            {
                Prop = "ba_prop_battle_hobby_horse",
                PropBone = 28422,
                PropPlacement = new object[] { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 },
                EmoteLoop = true,
            }),


            // DANCES END
            ["drink1"] = new AnimModel(new string("mp_player_inteat@pnq"), new string("loop"), new string("喝东西"), new AnimOptions
            {
                EmoteMoving = true,
                EmoteDuration = 2500,
            }),
            ["angry1"] = new AnimModel(new string("anim@mp_fm_event@intro"), new string("beast_transform"), new string("暴怒"), new AnimOptions
            {
                EmoteMoving = true,
                EmoteDuration = 5000,
            }),
            ["lay1"] = new AnimModel(new string("switch@trevor@scares_tramp"), new string("trev_scares_tramp_idle_tramp"), new string("享受躺"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["lay2"] = new AnimModel(new string("switch@trevor@annoys_sunbathers"), new string("trev_annoys_sunbathers_loop_girl"), new string("观云躺"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["lay3"] = new AnimModel(new string("switch@trevor@annoys_sunbathers"), new string("trev_annoys_sunbathers_loop_guy"), new string("观云躺 2"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["lay4"] = new AnimModel(new string("missfbi3_sniping"), new string("prone_dave"), new string("俯卧"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["sign1"] = new AnimModel(new string("misscarsteal3pullover"), new string("pull_over_right"), new string("停下"), new AnimOptions
            {
                EmoteMoving = true,
                EmoteDuration = 1300,
            }),
            ["stance1"] = new AnimModel(new string("anim@heists@heist_corona@team_idles@male_a"), new string("idle"), new string("姿态"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["stance2"] = new AnimModel(new string("amb@world_human_hang_out_street@male_b@idle_a"), new string("idle_b"), new string("姿态 8")),
            ["stance3"] = new AnimModel(new string("friends@fra@ig_1"), new string("base_idle"), new string("姿态 9"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["stance4"] = new AnimModel(new string("mp_move@prostitute@m@french"), new string("idle"), new string("姿态 10"), new AnimOptions
            {
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["stance5"] = new AnimModel(new string("random@countrysiderobbery"), new string("idle_a"), new string("姿态 11"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["stance6"] = new AnimModel(new string("anim@heists@heist_corona@team_idles@female_a"), new string("idle"), new string("姿态 2"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["stance7"] = new AnimModel(new string("anim@heists@humane_labs@finale@strip_club"), new string("ped_b_celebrate_loop"), new string("姿态 3"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["stance8"] = new AnimModel(new string("anim@mp_celebration@idles@female"), new string("celebration_idle_f_a"), new string("姿态 4"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["stance9"] = new AnimModel(new string("anim@mp_corona_idles@female_b@idle_a"), new string("idle_a"), new string("姿态 5"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["stance10"] = new AnimModel(new string("anim@mp_corona_idles@male_c@idle_a"), new string("idle_a"), new string("姿态 6"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["stance11"] = new AnimModel(new string("anim@mp_corona_idles@male_d@idle_a"), new string("idle_a"), new string("姿态 7"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["stance12"] = new AnimModel(new string("amb@world_human_hang_out_street@female_hold_arm@idle_a"), new string("idle_a"), new string("等待 3"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["drunk1"] = new AnimModel(new string("random@drunk_driver_1"), new string("drunk_driver_stand_loop_dd1"), new string("姿态 喝醉"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["drunk2"] = new AnimModel(new string("random@drunk_driver_1"), new string("drunk_driver_stand_loop_dd2"), new string("姿态 喝醉 2"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["drunk3"] = new AnimModel(new string("missarmenian2"), new string("standing_idle_loop_drunk"), new string("姿态 喝醉 3"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["airguitar"] = new AnimModel(new string("anim@mp_player_intcelebrationfemale@air_guitar"), new string("air_guitar"), new string("空气吉他")),
            ["joke"] = new AnimModel(new string("anim@mp_player_intcelebrationfemale@air_synth"), new string("air_synth"), new string("开玩笑")),
            ["angry2"] = new AnimModel(new string("misscarsteal4@actor"), new string("actor_berating_loop"), new string("生气"), new AnimOptions
            {
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["angry3"] = new AnimModel(new string("oddjobs@assassinate@vice@hooker"), new string("argue_a"), new string("生气 2"), new AnimOptions
            {
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["lean1"] = new AnimModel(new string("anim@amb@clubhouse@bar@drink@idle_a"), new string("idle_a_bartender"), new string("侧靠"), new AnimOptions
            {
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["kiss1"] = new AnimModel(new string("anim@mp_player_intcelebrationfemale@blow_kiss"), new string("blow_kiss"), new string("飞吻")),
            ["kiss2"] = new AnimModel(new string("anim@mp_player_intselfieblow_kiss"), new string("exit"), new string("飞吻 2"), new AnimOptions
            {
                EmoteMoving = true,
                EmoteDuration = 2000

            }),
            ["greetings1"] = new AnimModel(new string("anim@mp_player_intcelebrationpaired@f_f_sarcastic"), new string("sarcastic_left"), new string("打招呼")),
            ["greetings2"] = new AnimModel(new string("misscommon@response"), new string("bring_it_on"), new string("快来"), new AnimOptions
            {
                EmoteMoving = true,
                EmoteDuration = 3000
            }),
            ["sign2"] = new AnimModel(new string("mini@triathlon"), new string("want_some_of_this"), new string("来我这里"), new AnimOptions
            {
                EmoteMoving = true,
                EmoteDuration = 2000
            }),
            ["crossarms1"] = new AnimModel(new string("anim@amb@nightclub@peds@"), new string("rcmme_amanda1_stand_loop_cop"), new string("警察 2"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["radio1"] = new AnimModel(new string("amb@code_human_police_investigate@idle_a"), new string("idle_b"), new string("警察 3"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["crossarms2"] = new AnimModel(new string("amb@world_human_hang_out_street@female_arms_crossed@idle_a"), new string("idle_a"), new string("抱肘"), new AnimOptions
            {
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["crossarms3"] = new AnimModel(new string("amb@world_human_hang_out_street@male_c@idle_a"), new string("idle_b"), new string("抱肘 2"), new AnimOptions
            {
                EmoteMoving = true,
            }),
            ["crossarms4"] = new AnimModel(new string("anim@heists@heist_corona@single_team"), new string("single_team_loop_boss"), new string("抱肘 3"), new AnimOptions
            {
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["crossarms5"] = new AnimModel(new string("random@street_race"), new string("_car_b_lookout"), new string("抱肘 4"), new AnimOptions
            {
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["crossarms6"] = new AnimModel(new string("anim@amb@nightclub@peds@"), new string("rcmme_amanda1_stand_loop_cop"), new string("抱肘 5"), new AnimOptions
            {
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["crossarms7"] = new AnimModel(new string("anim@amb@nightclub@peds@"), new string("rcmme_amanda1_stand_loop_cop"), new string("抱肘 6"), new AnimOptions
            {
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["crossarms8"] = new AnimModel(new string("random@shop_gunstore"), new string("_idle"), new string("抱肘 7"), new AnimOptions
            {
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["crossarms9"] = new AnimModel(new string("anim@amb@business@bgen@bgen_no_work@"), new string("stand_phone_phoneputdown_idle_nowork"), new string("抱肘 8"), new AnimOptions
            {
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["crossarms10"] = new AnimModel(new string("rcmnigel1a_band_groupies"), new string("base_m2"), new string("抱肘 9"), new AnimOptions
            {
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["damn1"] = new AnimModel(new string("gestures@m@standing@casual"), new string("gesture_damn"), new string("糟糕"), new AnimOptions
            {
                EmoteMoving = true,
                EmoteDuration = 1000
            }),
            ["damn2"] = new AnimModel(new string("anim@am_hold_up@male"), new string("shoplift_mid"), new string("糟糕 2"), new AnimOptions
            {
                EmoteMoving = true,
                EmoteDuration = 1000
            }),
            ["sign3"] = new AnimModel(new string("gestures@f@standing@casual"), new string("gesture_hand_down"), new string("向下指"), new AnimOptions
            {
                EmoteMoving = true,
                EmoteDuration = 1000
            }),
            ["surrender"] = new AnimModel(new string("random@arrests@busted"), new string("idle_a"), new string("投降"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["facepalm1"] = new AnimModel(new string("anim@mp_player_intcelebrationfemale@face_palm"), new string("face_palm"), new string("捂脸 2"), new AnimOptions
            {
                EmoteMoving = true,
                EmoteDuration = 8000
            }),
            ["facepalm2"] = new AnimModel(new string("random@car_thief@agitated@idle_a"), new string("agitated_idle_a"), new string("捂脸"), new AnimOptions
            {
                EmoteMoving = true,
                EmoteDuration = 8000
            }),
            ["facepalm3"] = new AnimModel(new string("missminuteman_1ig_2"), new string("tasered_2"), new string("捂脸 3"), new AnimOptions
            {
                EmoteMoving = true,
                EmoteDuration = 8000
            }),
            ["facepalm4"] = new AnimModel(new string("anim@mp_player_intupperface_palm"), new string("idle_a"), new string("捂脸 4"), new AnimOptions
            {
                EmoteMoving = true,
                EmoteLoop = true,
            }),
            ["fall1"] = new AnimModel(new string("random@drunk_driver_1"), new string("drunk_fall_over"), new string("摔倒")),
            ["suicide1"] = new AnimModel(new string("mp_suicide"), new string("pistol"), new string("摔倒 2")),
            ["suicide2"] = new AnimModel(new string("mp_suicide"), new string("pill"), new string("摔倒 3")),
            ["fall2"] = new AnimModel(new string("friends@frf@ig_2"), new string("knockout_plyr"), new string("摔倒 4")),
            ["fall3"] = new AnimModel(new string("anim@gangops@hostage@"), new string("victim_fail"), new string("摔倒 5")),
            ["sleepwalker"] = new AnimModel(new string("mp_sleep"), new string("sleep_loop"), new string("入睡"), new AnimOptions
            {
                EmoteMoving = true,
                EmoteLoop = true,
            }),
            ["fight1"] = new AnimModel(new string("anim@deathmatch_intros@unarmed"), new string("intro_male_unarmed_c"), new string("挑衅")),
            ["fight2"] = new AnimModel(new string("anim@deathmatch_intros@unarmed"), new string("intro_male_unarmed_e"), new string("挑衅 2")),
            ["fucku1"] = new AnimModel(new string("anim@mp_player_intselfiethe_bird"), new string("idle_a"), new string("鄙视"), new AnimOptions
            {
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["fucku2"] = new AnimModel(new string("anim@mp_player_intupperfinger"), new string("idle_a_fp"), new string("鄙视 2"), new AnimOptions
            {
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["sign4"] = new AnimModel(new string("mp_ped_interaction"), new string("handshake_guy_a"), new string("握手"), new AnimOptions
            {
                EmoteMoving = true,
                EmoteDuration = 3000
            }),
            ["sign5"] = new AnimModel(new string("mp_ped_interaction"), new string("handshake_guy_b"), new string("握手 2"), new AnimOptions
            {
                EmoteMoving = true,
                EmoteDuration = 3000
            }),
            ["stance13"] = new AnimModel(new string("amb@world_human_hang_out_street@Female_arm_side@idle_a"), new string("idle_a"), new string("等待 4"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["stance14"] = new AnimModel(new string("missclothing"), new string("idle_storeclerk"), new string("等待 5"), new AnimOptions
            {
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["stance15"] = new AnimModel(new string("timetable@amanda@ig_2"), new string("ig_2_base_amanda"), new string("等待 6"), new AnimOptions
            {
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["stance16"] = new AnimModel(new string("rcmnigel1cnmt_1c"), new string("base"), new string("等待 7"), new AnimOptions
            {
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["stance17"] = new AnimModel(new string("rcmjosh1"), new string("idle"), new string("等待 8"), new AnimOptions
            {
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["stance18"] = new AnimModel(new string("rcmjosh2"), new string("josh_2_intp1_base"), new string("等待 9"), new AnimOptions
            {
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["stance19"] = new AnimModel(new string("timetable@amanda@ig_3"), new string("ig_3_base_tracy"), new string("等待 10"), new AnimOptions
            {
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["stance20"] = new AnimModel(new string("misshair_shop@hair_dressers"), new string("keeper_base"), new string("等待 11"), new AnimOptions
            {
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["carrybag"] = new AnimModel(new string("move_m@hiking"), new string("idle"), new string("旅行拿包"), new AnimOptions
            {
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["hug1"] = new AnimModel(new string("mp_ped_interaction"), new string("kisses_guy_a"), new string("拥抱")),
            ["hug2"] = new AnimModel(new string("mp_ped_interaction"), new string("kisses_guy_b"), new string("拥抱 2")),
            ["hug3"] = new AnimModel(new string("mp_ped_interaction"), new string("hugs_guy_a"), new string("拥抱 3")),
            ["inspect1"] = new AnimModel(new string("random@train_tracks"), new string("idle_e"), new string("检查")),
            ["sign6"] = new AnimModel(new string("anim@mp_player_intcelebrationfemale@jazz_hands"), new string("jazz_hands"), new string("爵士手势"), new AnimOptions
            {
                EmoteMoving = true,
                EmoteDuration = 6000,
            }),
            ["dance32"] = new AnimModel(new string("amb@world_human_jog_standing@male@idle_a"), new string("idle_a"), new string("漫步 2"), new AnimOptions
            {
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["dance33"] = new AnimModel(new string("amb@world_human_jog_standing@female@idle_a"), new string("idle_a"), new string("漫步 3"), new AnimOptions
            {
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["dance34"] = new AnimModel(new string("amb@world_human_power_walker@female@idle_a"), new string("idle_a"), new string("漫步 4"), new AnimOptions
            {
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["dance35"] = new AnimModel(new string("move_m@joy@a"), new string("walk"), new string("漫步 5"), new AnimOptions
            {
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["sport1"] = new AnimModel(new string("timetable@reunited@ig_2"), new string("jimmy_getknocked"), new string("Jumping Jacks"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["kneel1"] = new AnimModel(new string("rcmextreme3"), new string("idle"), new string("Kneel 2"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["kneel2"] = new AnimModel(new string("amb@world_human_bum_wash@male@low@idle_a"), new string("idle_a"), new string("Kneel 3"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["door1"] = new AnimModel(new string("timetable@jimmy@doorknock@"), new string("knockdoor_idle"), new string("Knock"), new AnimOptions
            {
                EmoteMoving = true,
                EmoteLoop = true,
            }),
            ["door2"] = new AnimModel(new string("missheistfbi3b_ig7"), new string("lift_fibagent_loop"), new string("Knock 2"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["fight3"] = new AnimModel(new string("anim@mp_player_intcelebrationfemale@knuckle_crunch"), new string("knuckle_crunch"), new string("Knuckle Crunch"), new AnimOptions
            {
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["strip1"] = new AnimModel(new string("mp_safehouse"), new string("lap_dance_girl"), new string("Lapdance")),
            ["lean2"] = new AnimModel(new string("amb@world_human_leaning@female@wall@back@hand_up@idle_a"), new string("idle_a"), new string("Lean 2"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["lean3"] = new AnimModel(new string("amb@world_human_leaning@female@wall@back@holding_elbow@idle_a"), new string("idle_a"), new string("Lean 3"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["lean4"] = new AnimModel(new string("amb@world_human_leaning@male@wall@back@foot_up@idle_a"), new string("idle_a"), new string("Lean 4"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["lean5"] = new AnimModel(new string("amb@world_human_leaning@male@wall@back@hands_together@idle_b"), new string("idle_b"), new string("Lean 5"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["sex1"] = new AnimModel(new string("random@street_race"), new string("_car_a_flirt_girl"), new string("Lean Flirt"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["lean6"] = new AnimModel(new string("amb@prop_human_bum_shopping_cart@male@idle_a"), new string("idle_c"), new string("Lean Bar 2"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["lean7"] = new AnimModel(new string("anim@amb@nightclub@lazlow@ig1_vip@"), new string("clubvip_base_laz"), new string("Lean Bar 3"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["lean8"] = new AnimModel(new string("anim@heists@prison_heist"), new string("ped_b_loop_a"), new string("Lean Bar 4"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["lean9"] = new AnimModel(new string("anim@mp_ferris_wheel"), new string("idle_a_player_one"), new string("Lean High"), new AnimOptions
            {
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["lean10"] = new AnimModel(new string("anim@mp_ferris_wheel"), new string("idle_a_player_two"), new string("Lean High 2"), new AnimOptions
            {
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["lean11"] = new AnimModel(new string("timetable@mime@01_gc"), new string("idle_a"), new string("Leanside"), new AnimOptions
            {
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["lean12"] = new AnimModel(new string("misscarstealfinale"), new string("packer_idle_1_trevor"), new string("Leanside 2"), new AnimOptions
            {
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["lean13"] = new AnimModel(new string("misscarstealfinalecar_5_ig_1"), new string("waitloop_lamar"), new string("Leanside 3"), new AnimOptions
            {
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["lean14"] = new AnimModel(new string("misscarstealfinalecar_5_ig_1"), new string("waitloop_lamar"), new string("Leanside 4"), new AnimOptions
            {
                EmoteLoop = true,
                EmoteMoving = false,
            }),
            ["lean15"] = new AnimModel(new string("rcmjosh2"), new string("josh_2_intp1_base"), new string("Leanside 5"), new AnimOptions
            {
                EmoteLoop = true,
                EmoteMoving = false,
            }),
            ["sign7"] = new AnimModel(new string("gestures@f@standing@casual"), new string("gesture_me_hard"), new string("Me"), new AnimOptions
            {
                EmoteMoving = true,
                EmoteDuration = 1000
            }),
            ["mechanic1"] = new AnimModel(new string("mini@repair"), new string("fixing_a_ped"), new string("Mechanic"), new AnimOptions
            {
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["mechanic2"] = new AnimModel(new string("amb@world_human_vehicle_mechanic@male@base"), new string("idle_a"), new string("Mechanic 2"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["mechanic3"] = new AnimModel(new string("anim@amb@clubhouse@tutorial@bkr_tut_ig3@"), new string("machinic_loop_mechandplayer"), new string("Mechanic 3"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["mechanic4"] = new AnimModel(new string("anim@amb@clubhouse@tutorial@bkr_tut_ig3@"), new string("machinic_loop_mechandplayer"), new string("Mechanic 4"), new AnimOptions
            {
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["inspect2"] = new AnimModel(new string("amb@medic@standing@tendtodead@base"), new string("base"), new string("Medic 2"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["meditate1"] = new AnimModel(new string("rcmcollect_paperleadinout@"), new string("meditiate_idle"), new string("Meditiate"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["meditate2"] = new AnimModel(new string("rcmepsilonism3"), new string("ep_3_rcm_marnie_meditating"), new string("Meditiate 2"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["meditate3"] = new AnimModel(new string("rcmepsilonism3"), new string("base_loop"), new string("Meditiate 3"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["sign8"] = new AnimModel(new string("anim@mp_player_intincarrockstd@ps@"), new string("idle_a"), new string("Metal"), new AnimOptions
            {
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["no1"] = new AnimModel(new string("anim@heists@ornate_bank@chat_manager"), new string("fail"), new string("No"), new AnimOptions
            {
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["no2"] = new AnimModel(new string("mp_player_int_upper_nod"), new string("mp_player_int_nod_no"), new string("No 2"), new AnimOptions
            {
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["nosepick"] = new AnimModel(new string("anim@mp_player_intcelebrationfemale@nose_pick"), new string("nose_pick"), new string("Nose Pick"), new AnimOptions
            {
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["no3"] = new AnimModel(new string("gestures@m@standing@casual"), new string("gesture_no_way"), new string("No Way"), new AnimOptions
            {
                EmoteDuration = 1500,
                EmoteMoving = true,
            }),
            ["ok"] = new AnimModel(new string("anim@mp_player_intselfiedock"), new string("idle_a"), new string("OK"), new AnimOptions
            {
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["exhaust"] = new AnimModel(new string("re@construction"), new string("out_of_breath"), new string("Out of Breath"), new AnimOptions
            {
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["pickup"] = new AnimModel(new string("random@domestic"), new string("pickup_low"), new string("Pickup")),
            ["push1"] = new AnimModel(new string("missfinale_c2ig_11"), new string("pushcar_offcliff_f"), new string("Push"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["push2"] = new AnimModel(new string("missfinale_c2ig_11"), new string("pushcar_offcliff_m"), new string("Push 2"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["sign9"] = new AnimModel(new string("gestures@f@standing@casual"), new string("gesture_point"), new string("Point"), new AnimOptions
            {
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["sport2"] = new AnimModel(new string("amb@world_human_push_ups@male@idle_a"), new string("idle_d"), new string("Pushup"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["clap1"] = new AnimModel(new string("random@street_race"), new string("grid_girl_race_start"), new string("Countdown"), new AnimOptions
            {
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["sign10"] = new AnimModel(new string("mp_gun_shop_tut"), new string("indicate_right"), new string("Point Right"), new AnimOptions
            {
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["salute1"] = new AnimModel(new string("anim@mp_player_intincarsalutestd@ds@"), new string("idle_a"), new string("Salute"), new AnimOptions
            {
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["salute2"] = new AnimModel(new string("anim@mp_player_intincarsalutestd@ps@"), new string("idle_a"), new string("Salute 2"), new AnimOptions
            {
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["salute3"] = new AnimModel(new string("anim@mp_player_intuppersalute"), new string("idle_a"), new string("Salute 3"), new AnimOptions
            {
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["scared1"] = new AnimModel(new string("random@domestic"), new string("f_distressed_loop"), new string("Scared"), new AnimOptions
            {
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["scared2"] = new AnimModel(new string("random@homelandsecurity"), new string("knees_loop_girl"), new string("Scared 2"), new AnimOptions
            {
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["fucku3"] = new AnimModel(new string("misscommon@response"), new string("screw_you"), new string("Screw You"), new AnimOptions
            {
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["shake"] = new AnimModel(new string("move_m@_idles@shake_off"), new string("shakeoff_1"), new string("Shake Off"), new AnimOptions
            {
                EmoteMoving = true,
                EmoteDuration = 3500,
            }),
            ["injured1"] = new AnimModel(new string("random@dealgonewrong"), new string("idle_a"), new string("Shot"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["sleep1"] = new AnimModel(new string("timetable@tracy@sleep@"), new string("idle_c"), new string("Sleep"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["sign11"] = new AnimModel(new string("gestures@f@standing@casual"), new string("gesture_shrug_hard"), new string("Shrug"), new AnimOptions
            {
                EmoteMoving = true,
                EmoteDuration = 1000,
            }),
            ["sign12"] = new AnimModel(new string("gestures@m@standing@casual"), new string("gesture_shrug_hard"), new string("Shrug 2"), new AnimOptions
            {
                EmoteMoving = true,
                EmoteDuration = 1000,
            }),
            ["sit1"] = new AnimModel(new string("anim@amb@business@bgen@bgen_no_work@"), new string("sit_phone_phoneputdown_idle_nowork"), new string("Sit"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["sit2"] = new AnimModel(new string("rcm_barry3"), new string("barry_3_sit_loop"), new string("Sit 2"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["sit3"] = new AnimModel(new string("amb@world_human_picnic@male@idle_a"), new string("idle_a"), new string("Sit 3"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["sit4"] = new AnimModel(new string("amb@world_human_picnic@female@idle_a"), new string("idle_a"), new string("Sit 4"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["sit5"] = new AnimModel(new string("anim@heists@fleeca_bank@ig_7_jetski_owner"), new string("owner_idle"), new string("Sit 5"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["sit6"] = new AnimModel(new string("timetable@jimmy@mics3_ig_15@"), new string("idle_a_jimmy"), new string("Sit 6"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["sit7"] = new AnimModel(new string("anim@amb@nightclub@lazlow@lo_alone@"), new string("lowalone_base_laz"), new string("Sit 7"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["sit8"] = new AnimModel(new string("timetable@jimmy@mics3_ig_15@"), new string("mics3_15_base_jimmy"), new string("Sit 8"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["sit9"] = new AnimModel(new string("amb@world_human_stupor@male@idle_a"), new string("idle_a"), new string("Sit 9"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["sit10"] = new AnimModel(new string("timetable@tracy@ig_14@"), new string("ig_14_base_tracy"), new string("Sit Lean"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["sit11"] = new AnimModel(new string("anim@amb@business@bgen@bgen_no_work@"), new string("sit_phone_phoneputdown_sleeping-noworkfemale"), new string("Sit Sad"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["sit12"] = new AnimModel(new string("anim@heists@ornate_bank@hostages@hit"), new string("hit_loop_ped_b"), new string("Sit Scared"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["pdTeckle"] = new AnimModel(new string("missmic2ig_11"), new string("mic_2_ig_11_intro_goon"), new string("Pd Teckle"), new AnimOptions
            {
                EmoteLoop = false,
            }),
            ["pdTeckleVictim"] = new AnimModel(new string("missmic2ig_11"), new string("mic_2_ig_11_intro_p_one"), new string("Pd Teckle 2"), new AnimOptions
            {
                EmoteLoop = false,
            }),
            ["scared3"] = new AnimModel(new string("anim@heists@ornate_bank@hostages@ped_c@"), new string("flinch_loop"), new string("Sit Scared 2"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["scared4"] = new AnimModel(new string("anim@heists@ornate_bank@hostages@ped_e@"), new string("flinch_loop"), new string("Sit Scared 3"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["drunk4"] = new AnimModel(new string("timetable@amanda@drunk@base"), new string("base"), new string("Sit Drunk"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["chair1"] = new AnimModel(new string("timetable@ron@ig_5_p3"), new string("ig_5_p3_base"), new string("Sit Chair 2"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["chair2"] = new AnimModel(new string("timetable@reunited@ig_10"), new string("base_amanda"), new string("Sit Chair 3"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["chair3"] = new AnimModel(new string("timetable@ron@ig_3_couch"), new string("base"), new string("Sit Chair 4"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["chair4"] = new AnimModel(new string("timetable@jimmy@mics3_ig_15@"), new string("mics3_15_base_tracy"), new string("Sit Chair 5"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["chair5"] = new AnimModel(new string("timetable@maid@couch@"), new string("base"), new string("Sit Chair 6"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["chair6"] = new AnimModel(new string("timetable@ron@ron_ig_2_alt1"), new string("ig_2_alt1_base"), new string("Sit Chair Side"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["sport3"] = new AnimModel(new string("amb@world_human_sit_ups@male@idle_a"), new string("idle_a"), new string("Sit Up"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["fucku4"] = new AnimModel(new string("anim@arena@celeb@flat@solo@no_props@"), new string("angry_clap_a_player_a"), new string("Clap Angry"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["clap2"] = new AnimModel(new string("anim@mp_player_intupperslow_clap"), new string("idle_a"), new string("Slow Clap 3"), new AnimOptions
            {
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["clap3"] = new AnimModel(new string("amb@world_human_cheering@male_a"), new string("base"), new string("Clap"), new AnimOptions
            {
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["clap4"] = new AnimModel(new string("anim@mp_player_intcelebrationfemale@slow_clap"), new string("slow_clap"), new string("Slow Clap"), new AnimOptions
            {
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["clap5"] = new AnimModel(new string("anim@mp_player_intcelebrationmale@slow_clap"), new string("slow_clap"), new string("Slow Clap 2"), new AnimOptions
            {
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["smell"] = new AnimModel(new string("move_p_m_two_idles@generic"), new string("fidget_sniff_fingers"), new string("Smell"), new AnimOptions
            {
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["aim1"] = new AnimModel(new string("random@countryside_gang_fight"), new string("biker_02_stickup_loop"), new string("Stick Up"), new AnimOptions
            {
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["drunk5"] = new AnimModel(new string("misscarsteal4@actor"), new string("stumble"), new string("Stumble"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["shock"] = new AnimModel(new string("stungun@standing"), new string("damage"), new string("Stunned"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["lay5"] = new AnimModel(new string("amb@world_human_sunbathe@male@back@base"), new string("base"), new string("Sunbathe"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["lay6"] = new AnimModel(new string("amb@world_human_sunbathe@female@back@base"), new string("base"), new string("Sunbathe 2"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["stance21"] = new AnimModel(new string("missfam5_yoga"), new string("a2_pose"), new string("T"), new AnimOptions
            {
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["stance22"] = new AnimModel(new string("mp_sleep"), new string("bind_pose_180"), new string("T 2"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["head"] = new AnimModel(new string("mp_cp_welcome_tutthink"), new string("b_think"), new string("Think 5"), new AnimOptions
            {
                EmoteMoving = true,
                EmoteDuration = 2000,
            }),
            ["think1"] = new AnimModel(new string("misscarsteal4@aliens"), new string("rehearsal_base_idle_director"), new string("Think"), new AnimOptions
            {
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["think2"] = new AnimModel(new string("timetable@tracy@ig_8@base"), new string("base"), new string("Think 3"), new AnimOptions
            {
                EmoteLoop = true,
                EmoteMoving = true,
            }),

            ["think3"] = new AnimModel(new string("missheist_jewelleadinout"), new string("jh_int_outro_loop_a"), new string("Think 2"), new AnimOptions
            {
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["thumbsup1"] = new AnimModel(new string("anim@mp_player_intincarthumbs_uplow@ds@"), new string("enter"), new string("Thumbs Up 3"), new AnimOptions
            {
                EmoteMoving = true,
                EmoteDuration = 3000,
            }),
            ["thumbsup2"] = new AnimModel(new string("anim@mp_player_intselfiethumbs_up"), new string("idle_a"), new string("Thumbs Up 2"), new AnimOptions
            {
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["thumbsup3"] = new AnimModel(new string("anim@mp_player_intupperthumbs_up"), new string("idle_a"), new string("Thumbs Up"), new AnimOptions
            {
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["type1"] = new AnimModel(new string("anim@heists@prison_heiststation@cop_reactions"), new string("cop_b_idle"), new string("Type"), new AnimOptions
            {
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["type2"] = new AnimModel(new string("anim@heists@prison_heistig1_p1_guard_checks_bus"), new string("loop"), new string("Type 2"), new AnimOptions
            {
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["type3"] = new AnimModel(new string("mp_prison_break"), new string("hack_loop"), new string("Type 3"), new AnimOptions
            {
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["type4"] = new AnimModel(new string("mp_fbi_heist"), new string("loop"), new string("Type 4"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["heat"] = new AnimModel(new string("amb@world_human_stand_fire@male@idle_a"), new string("idle_a"), new string("Warmth"), new AnimOptions
            {
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["wave1"] = new AnimModel(new string("random@mugging5"), new string("001445_01_gangintimidation_1_female_idle_b"), new string("Wave 4"), new AnimOptions
            {
                EmoteMoving = true,
                EmoteDuration = 3000,
            }),
            ["wave2"] = new AnimModel(new string("anim@mp_player_intcelebrationfemale@wave"), new string("wave"), new string("Wave 2"), new AnimOptions
            {
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["wave3"] = new AnimModel(new string("friends@fra@ig_1"), new string("over_here_idle_a"), new string("Wave 3"), new AnimOptions
            {
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["wave4"] = new AnimModel(new string("friends@frj@ig_1"), new string("wave_a"), new string("Wave"), new AnimOptions
            {
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["wave5"] = new AnimModel(new string("friends@frj@ig_1"), new string("wave_b"), new string("Wave 5"), new AnimOptions
            {
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["wave6"] = new AnimModel(new string("friends@frj@ig_1"), new string("wave_c"), new string("Wave 6"), new AnimOptions
            {
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["wave7"] = new AnimModel(new string("friends@frj@ig_1"), new string("wave_d"), new string("Wave 7"), new AnimOptions
            {
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["wave8"] = new AnimModel(new string("friends@frj@ig_1"), new string("wave_e"), new string("Wave 8"), new AnimOptions
            {
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["wave9"] = new AnimModel(new string("gestures@m@standing@casual"), new string("gesture_hello"), new string("Wave 9"), new AnimOptions
            {
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["whistle1"] = new AnimModel(new string("taxi_hail"), new string("hail_taxi"), new string("Whistle"), new AnimOptions
            {
                EmoteMoving = true,
                EmoteDuration = 1300,
            }),
            ["whistle2"] = new AnimModel(new string("rcmnigel1c"), new string("hailing_whistle_waive_a"), new string("Whistle 2"), new AnimOptions
            {
                EmoteMoving = true,
                EmoteDuration = 2000,
            }),
            ["fucku5"] = new AnimModel(new string("anim@mp_player_intupperair_shagging"), new string("idle_a"), new string("Yeah"), new AnimOptions
            {
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["hitchhiking"] = new AnimModel(new string("random@hitch_lift"), new string("idle_f"), new string("Lift"), new AnimOptions
            {
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["sign13"] = new AnimModel(new string("anim@arena@celeb@flat@paired@no_props@"), new string("laugh_a_player_b"), new string("LOL"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["sign14"] = new AnimModel(new string("anim@arena@celeb@flat@solo@no_props@"), new string("giggle_a_player_b"), new string("LOL 2"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["stance23"] = new AnimModel(new string("fra_0_int-1"), new string("cs_lamardavis_dual-1"), new string("Statue 2"), new AnimOptions
            {
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["stance24"] = new AnimModel(new string("club_intro2-0"), new string("csb_englishdave_dual-0"), new string("Statue 3"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["gangsign1"] = new AnimModel(new string("mp_player_int_uppergang_sign_a"), new string("mp_player_int_gang_sign_a"), new string("Gang Sign"), new AnimOptions
            {
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["gangsign2"] = new AnimModel(new string("mp_player_int_uppergang_sign_b"), new string("mp_player_int_gang_sign_b"), new string("Gang Sign 2"), new AnimOptions
            {
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["sleep2"] = new AnimModel(new string("missarmenian2"), new string("drunk_loop"), new string("Passout"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["sleep3"] = new AnimModel(new string("missarmenian2"), new string("corpse_search_exit_ped"), new string("Passout 2"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["sleep4"] = new AnimModel(new string("anim@gangops@morgue@table@"), new string("body_search"), new string("Passout 3"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["sleep5"] = new AnimModel(new string("mini@cpr@char_b@cpr_def"), new string("cpr_pumpchest_idle"), new string("Passout 4"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["lay7"] = new AnimModel(new string("random@mugging4"), new string("flee_backward_loop_shopkeeper"), new string("Passout 5"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["mechanic5"] = new AnimModel(new string("creatures@rottweiler@tricks@"), new string("petting_franklin"), new string("Petting"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["injured2"] = new AnimModel(new string("move_injured_ground"), new string("front_loop"), new string("Crawl"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["sport4"] = new AnimModel(new string("anim@arena@celeb@flat@solo@no_props@"), new string("cap_a_player_a"), new string("Flip 2")),
            ["sport5"] = new AnimModel(new string("anim@arena@celeb@flat@solo@no_props@"), new string("flip_a_player_a"), new string("Flip")),
            ["slide1"] = new AnimModel(new string("anim@arena@celeb@flat@solo@no_props@"), new string("slide_a_player_a"), new string("Slide")),
            ["slide2"] = new AnimModel(new string("anim@arena@celeb@flat@solo@no_props@"), new string("slide_b_player_a"), new string("Slide 2")),
            ["slide3"] = new AnimModel(new string("anim@arena@celeb@flat@solo@no_props@"), new string("slide_c_player_a"), new string("Slide 3")),
            ["bat1"] = new AnimModel(new string("anim@arena@celeb@flat@solo@no_props@"), new string("slugger_a_player_a"), new string("Slugger")),
            ["fucku6"] = new AnimModel(new string("anim@arena@celeb@podium@no_prop@"), new string("flip_off_a_1st"), new string("Flip Off"), new AnimOptions
            {
                EmoteMoving = true,
            }),
            ["fucku7"] = new AnimModel(new string("anim@arena@celeb@podium@no_prop@"), new string("flip_off_c_1st"), new string("Flip Off 2"), new AnimOptions
            {
                EmoteMoving = true,
            }),
            ["greetings3"] = new AnimModel(new string("anim@arena@celeb@podium@no_prop@"), new string("regal_c_1st"), new string("Bow"), new AnimOptions
            {
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["greetings4"] = new AnimModel(new string("anim@arena@celeb@podium@no_prop@"), new string("regal_a_1st"), new string("Bow 2"), new AnimOptions
            {
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["carkey"] = new AnimModel(new string("anim@mp_player_intmenu@key_fob@"), new string("fob_click"), new string("Key Fob"), new AnimOptions
            {
                EmoteLoop = false,
                EmoteMoving = true,
                EmoteDuration = 1000,
            }),
            ["bat2"] = new AnimModel(new string("rcmnigel1d"), new string("swing_a_mark"), new string("Golf Swing")),
            ["eat"] = new AnimModel(new string("mp_player_inteat@burger"), new string("mp_player_int_eat_burger"), new string("Eat"), new AnimOptions
            {
                EmoteMoving = true,
                EmoteDuration = 3000,
            }),
            ["cop"] = new AnimModel(new string("move_m@intimidation@cop@unarmed"), new string("idle"), new string("Reaching"), new AnimOptions
            {
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["stance25"] = new AnimModel(new string("random@shop_tattoo"), new string("_idle_a"), new string("Wait"), new AnimOptions
            {
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["stance26"] = new AnimModel(new string("missbigscore2aig_3"), new string("wait_for_van_c"), new string("Wait 2"), new AnimOptions
            {
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["stance27"] = new AnimModel(new string("rcmjosh1"), new string("idle"), new string("Wait 12"), new AnimOptions
            {
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["stance28"] = new AnimModel(new string("rcmnigel1a"), new string("base"), new string("Wait 13"), new AnimOptions
            {
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["strip2"] = new AnimModel(new string("mini@strip_club@private_dance@idle"), new string("priv_dance_idle"), new string("Lap舞蹈 2"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["strip3"] = new AnimModel(new string("mini@strip_club@private_dance@part2"), new string("priv_dance_p2"), new string("Lap舞蹈 3"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["strip4"] = new AnimModel(new string("mini@strip_club@private_dance@part3"), new string("priv_dance_p3"), new string("Lap舞蹈 3"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["sex2"] = new AnimModel(new string("switch@trevor@mocks_lapdance"), new string("001443_01_trvs_28_idle_stripper"), new string("Twerk"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["fight4"] = new AnimModel(new string("melee@unarmed@streamed_variations"), new string("plyr_takedown_front_slap"), new string("Slap"), new AnimOptions
            {
                EmoteLoop = true,
                EmoteMoving = true,
                EmoteDuration = 2000,
            }),
            ["fight5"] = new AnimModel(new string("melee@unarmed@streamed_variations"), new string("plyr_takedown_front_headbutt"), new string("Headbutt")),
            ["dance36"] = new AnimModel(new string("anim@mp_player_intupperfind_the_fish"), new string("idle_a"), new string("Fish Dance"), new AnimOptions
            {
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["sign15"] = new AnimModel(new string("mp_player_int_upperpeace_sign"), new string("mp_player_int_peace_sign"), new string("Peace"), new AnimOptions
            {
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["sign16"] = new AnimModel(new string("anim@mp_player_intupperpeace"), new string("idle_a"), new string("Peace 2"), new AnimOptions
            {
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["medic1"] = new AnimModel(new string("mini@cpr@char_a@cpr_str"), new string("cpr_pumpchest"), new string("CPR"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["medic2"] = new AnimModel(new string("mini@cpr@char_a@cpr_str"), new string("cpr_pumpchest"), new string("CPR 2"), new AnimOptions
            {
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["sex3"] = new AnimModel(new string("missfbi1"), new string("ledge_loop"), new string("Ledge"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["sex4"] = new AnimModel(new string("missfbi1"), new string("ledge_loop"), new string("Air Plane"), new AnimOptions
            {
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["push3"] = new AnimModel(new string("random@paparazzi@peek"), new string("left_peek_a"), new string("Peek"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["ehm"] = new AnimModel(new string("timetable@gardener@smoking_joint"), new string("idle_cough"), new string("Cough"), new AnimOptions
            {
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["sport6"] = new AnimModel(new string("mini@triathlon"), new string("idle_e"), new string("Stretch"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["sport7"] = new AnimModel(new string("mini@triathlon"), new string("idle_f"), new string("Stretch 2"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["sport8"] = new AnimModel(new string("mini@triathlon"), new string("idle_d"), new string("Stretch 3"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["sport9"] = new AnimModel(new string("rcmfanatic1maryann_stretchidle_b"), new string("idle_e"), new string("Stretch 4"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["slide4"] = new AnimModel(new string("rcmfanatic1celebrate"), new string("celebrate"), new string("Celebrate"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["sport10"] = new AnimModel(new string("rcmextreme2"), new string("loop_punching"), new string("Punching"), new AnimOptions
            {
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["stance29"] = new AnimModel(new string("rcmbarry"), new string("base"), new string("Superhero"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["stance30"] = new AnimModel(new string("rcmbarry"), new string("base"), new string("Superhero 2"), new AnimOptions
            {
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["force1"] = new AnimModel(new string("rcmbarry"), new string("mind_control_b_loop"), new string("Mind Control"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["force2"] = new AnimModel(new string("rcmbarry"), new string("bar_1_attack_idle_aln"), new string("Mind Control 2"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["clown1"] = new AnimModel(new string("rcm_barry2"), new string("clown_idle_0"), new string("Clown"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["clown2"] = new AnimModel(new string("rcm_barry2"), new string("clown_idle_1"), new string("Clown 2"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["clown3"] = new AnimModel(new string("rcm_barry2"), new string("clown_idle_2"), new string("Clown 3"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["clown4"] = new AnimModel(new string("rcm_barry2"), new string("clown_idle_3"), new string("Clown 4"), new AnimOptions
            {
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["clown5"] = new AnimModel(new string("rcm_barry2"), new string("clown_idle_6"), new string("Clown 5"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["mirror1"] = new AnimModel(new string("mp_clothing@female@trousers"), new string("try_trousers_neutral_a"), new string("Try Clothes"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["mirror2"] = new AnimModel(new string("mp_clothing@female@shirt"), new string("try_shirt_positive_a"), new string("Try Clothes 2"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["mirror3"] = new AnimModel(new string("mp_clothing@female@shoes"), new string("try_shoes_positive_a"), new string("Try Clothes 3"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["stance31"] = new AnimModel(new string("mp_missheist_countrybank@nervous"), new string("nervous_idle"), new string("Nervous 2"), new AnimOptions
            {
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["stance32"] = new AnimModel(new string("amb@world_human_bum_standing@twitchy@idle_a"), new string("idle_c"), new string("Nervous"), new AnimOptions
            {
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["stance33"] = new AnimModel(new string("rcmme_tracey1"), new string("nervous_loop"), new string("Nervous 3"), new AnimOptions
            {
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["ring"] = new AnimModel(new string("mp_arresting"), new string("a_uncuff"), new string("Uncuff"), new AnimOptions
            {
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["pray"] = new AnimModel(new string("timetable@amanda@ig_4"), new string("ig_4_base"), new string("Namaste"), new AnimOptions
            {
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["dj"] = new AnimModel(new string("anim@amb@nightclub@djs@dixon@"), new string("dixn_dance_cntr_open_dix"), new string("DJ"), new AnimOptions
            {
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["hug4"] = new AnimModel(new string("random@atmrobberygen"), new string("b_atm_mugging"), new string("Threaten"), new AnimOptions
            {
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["radio2"] = new AnimModel(new string("random@arrests"), new string("generic_radio_chatter"), new string("Radio"), new AnimOptions
            {
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["pull"] = new AnimModel(new string("random@mugging4"), new string("struggle_loop_b_thief"), new string("Pull"), new AnimOptions
            {
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["greetings5"] = new AnimModel(new string("random@peyote@bird"), new string("wakeup"), new string("Bird")),
            ["dance37"] = new AnimModel(new string("random@peyote@chicken"), new string("wakeup"), new string("Chicken"), new AnimOptions
            {
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["sex5"] = new AnimModel(new string("random@peyote@dog"), new string("wakeup"), new string("Bark")),
            ["rabbit"] = new AnimModel(new string("random@peyote@rabbit"), new string("wakeup"), new string("Rabbit")),
            ["sex6"] = new AnimModel(new string("missexile3"), new string("ex03_train_roof_idle"), new string("Spider-Man"), new AnimOptions
            {
                EmoteLoop = true,
            }),
            ["sign17"] = new AnimModel(new string("special_ped@jane@monologue_5@monologue_5c"), new string("brotheradrianhasshown_2"), new string("BOI"), new AnimOptions
            {
                EmoteMoving = true,
                EmoteDuration = 3000,
            }),
            ["fight6"] = new AnimModel(new string("missmic4"), new string("michael_tux_fidget"), new string("Adjust"), new AnimOptions
            {
                EmoteMoving = true,
                EmoteDuration = 4000,
            }),
            ["handsup"] = new AnimModel(new string("missminuteman_1ig_2"), new string("handsup_base"), new string("Hands Up"), new AnimOptions
            {
                EmoteMoving = true,
                EmoteLoop = true,
            }),
            ["silahbele"] = new AnimModel(new string("reaction@intimidation@1h"), "outro", "silahbele", new AnimOptions
            {
                EmoteMoving = true,
            }),
            ["silahele"] = new AnimModel("reaction@intimidation@1h", "intro", "silahele", new AnimOptions
            {
                EmoteMoving = true,
            }),

            // Objeli Animler

            ["karate1"] = new AnimModel(new string("anim@mp_player_intcelebrationfemale@karate_chops"), new string("karate_chops"), new string("Karate")),
            ["karate2"] = new AnimModel(new string("anim@mp_player_intcelebrationmale@karate_chops"), new string("karate_chops"), new string("Karate 2")),
            ["threat1"] = new AnimModel(new string("anim@mp_player_intcelebrationmale@cut_throat"), new string("cut_throat"), new string("Cut Throat")),
            ["threat2"] = new AnimModel(new string("anim@mp_player_intcelebrationfemale@cut_throat"), new string("cut_throat"), new string("Cut Throat 2")),
            ["greetings6"] = new AnimModel(new string("anim@mp_player_intcelebrationmale@mind_blown"), new string("mind_blown"), new string("Mind Blown"), new AnimOptions
            {
                EmoteMoving = true,
                EmoteDuration = 4000
            }),
            ["greetings7"] = new AnimModel(new string("anim@mp_player_intcelebrationfemale@mind_blown"), new string("mind_blown"), new string("Mind Blown 2"), new AnimOptions
            {
                EmoteMoving = true,
                EmoteDuration = 4000
            }),
            ["boxing1"] = new AnimModel(new string("anim@mp_player_intcelebrationmale@shadow_boxing"), new string("shadow_boxing"), new string("Boxing"), new AnimOptions
            {
                EmoteMoving = true,
                EmoteDuration = 4000
            }),
            ["boxing2"] = new AnimModel(new string("anim@mp_player_intcelebrationfemale@shadow_boxing"), new string("shadow_boxing"), new string("Boxing 2"), new AnimOptions
            {
                EmoteMoving = true,
                EmoteDuration = 4000
            }),
            ["no4"] = new AnimModel(new string("anim@mp_player_intcelebrationfemale@stinker"), new string("stinker"), new string("Stink"), new AnimOptions
            {
                EmoteMoving = true,
                EmoteLoop = true
            }),
            ["think4"] = new AnimModel(new string("anim@amb@casino@hangout@ped_male@stand@02b@idles"), new string("idle_a"), new string("Think 4"), new AnimOptions
            {
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["stance34"] = new AnimModel(new string("clothingtie"), new string("try_tie_positive_a"), new string("Adjust Tie"), new AnimOptions
            {
                EmoteMoving = true,
                EmoteDuration = 5000
            }),

            ["umbrella"] = new AnimModel(new string("amb@world_human_drinking@coffee@male@base"), new string("base"), new string("Umbrella"), new AnimOptions
            {
                Prop = "p_amb_brolly_01",
                PropBone = 57005,
                PropPlacement = new object[] { 0.15, 0.005, 0.0, 87.0, -20.0, 180.0 },
                EmoteLoop = true,
                EmoteMoving = true,
            }),


            ["notepad"] = new AnimModel(new string("missheistdockssetup1clipboard@base"), new string("base"), new string("Notepad"), new AnimOptions
            {
                Prop = "prop_notepad_01",
                PropBone = 18905,
                PropPlacement = new object[] { 0.1, 0.02, 0.05, 10.0, 0.0, 0.0 },
                SecondProp = "prop_pencil_01",
                SecondPropBone = 58866,
                SecondPropPlacement = new object[] { 0.11, -0.02, 0.001, -120.0, 0.0, 0.0 },
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["carrybox"] = new AnimModel(new string("anim@heists@box_carry@"), new string("idle"), new string("Box"), new AnimOptions
            {
                Prop = "hei_prop_heist_box",
                PropBone = 60309,
                PropPlacement = new object[] { 0.025, 0.08, 0.255, -145.0, 290.0, 0.0 },
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["rose"] = new AnimModel(new string("anim@heists@humane_labs@finale@keycards"), new string("ped_a_enter_loop"), new string("Rose"), new AnimOptions
            {
                Prop = "prop_single_rose",
                PropBone = 18905,
                PropPlacement = new object[] { 0.13, 0.15, 0.0, -100.0, 0.0, -20.0 },
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["smoke1"] = new AnimModel(new string("amb@world_human_aa_smoke@male@idle_a"), new string("idle_c"), new string("Smoke 2"), new AnimOptions
            {
                Prop = "prop_cs_ciggy_01",
                PropBone = 28422,
                PropPlacement = new object[] { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 },
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["smoke2"] = new AnimModel(new string("amb@world_human_aa_smoke@male@idle_a"), new string("idle_b"), new string("Smoke 3"), new AnimOptions
            {
                Prop = "prop_cs_ciggy_01",
                PropBone = 28422,
                PropPlacement = new object[] { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 },
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["smoke3"] = new AnimModel(new string("amb@world_human_smoking@female@idle_a"), new string("idle_b"), new string("Smoke 4"), new AnimOptions
            {
                Prop = "prop_cs_ciggy_01",
                PropBone = 28422,
                PropPlacement = new object[] { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 },
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["bong"] = new AnimModel(new string("anim@safehouse@bong"), new string("bong_stage3"), new string("Bong"), new AnimOptions
            {
                Prop = "hei_heist_sh_bong_01",
                PropBone = 18905,
                PropPlacement = new object[] { 0.10, -0.25, 0.0, 95.0, 190.0, 180.0 },
            }),
            ["bag1"] = new AnimModel(new string("missheistdocksprep1hold_cellphone"), new string("static"), new string("Suitcase"), new AnimOptions
            {
                Prop = "prop_ld_suitcase_01",
                PropBone = 57005,
                PropPlacement = new object[] { 0.39, 0.0, 0.0, 0.0, 266.0, 60.0 },
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["bag2"] = new AnimModel(new string("missheistdocksprep1hold_cellphone"), new string("static"), new string("Suitcase 2"), new AnimOptions
            {
                Prop = "prop_security_case_01",
                PropBone = 57005,
                PropPlacement = new object[] { 0.10, 0.0, 0.0, 0.0, 280.0, 53.0 },
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["bugshot"] = new AnimModel(new string("mp_character_creation@customise@male_a"), new string("loop"), new string("Mugshot"), new AnimOptions
            {
                Prop = "prop_police_id_board",
                PropBone = 58868,
                PropPlacement = new object[] { 0.12, 0.24, 0.0, 5.0, 0.0, 70.0 },
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["coffee"] = new AnimModel(new string("amb@world_human_drinking@coffee@male@idle_a"), new string("idle_c"), new string("Coffee"), new AnimOptions
            {
                Prop = "p_amb_coffeecup_01",
                PropBone = 28422,
                PropPlacement = new object[] { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 },
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["whiskey"] = new AnimModel(new string("amb@world_human_drinking@coffee@male@idle_a"), new string("idle_c"), new string("Whiskey"), new AnimOptions
            {
                Prop = "prop_drink_whisky",
                PropBone = 28422,
                PropPlacement = new object[] { 0.01, -0.01, -0.06, 0.0, 0.0, 0.0 },
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["beer"] = new AnimModel(new string("amb@world_human_drinking@coffee@male@idle_a"), new string("idle_c"), new string("Beer"), new AnimOptions
            {
                Prop = "prop_amb_beer_bottle",
                PropBone = 28422,
                PropPlacement = new object[] { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 },
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["cup"] = new AnimModel(new string("amb@world_human_drinking@coffee@male@idle_a"), new string("idle_c"), new string("Cup"), new AnimOptions
            {
                Prop = "prop_plastic_cup_02",
                PropBone = 28422,
                PropPlacement = new object[] { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 },
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["donut"] = new AnimModel(new string("mp_player_inteat@burger"), new string("mp_player_int_eat_burger"), new string("Donut"), new AnimOptions
            {
                Prop = "prop_amb_donut",
                PropBone = 18905,
                PropPlacement = new object[] { 0.13, 0.05, 0.02, -50.0, 16.0, 60.0 },
                EmoteMoving = true,
            }),
            ["burger"] = new AnimModel(new string("mp_player_inteat@burger"), new string("mp_player_int_eat_burger"), new string("Burger"), new AnimOptions
            {
                Prop = "prop_cs_burger_01",
                PropBone = 18905,
                PropPlacement = new object[] { 0.13, 0.05, 0.02, -50.0, 16.0, 60.0 },
                EmoteMoving = true,
            }),
            ["sandwich"] = new AnimModel(new string("mp_player_inteat@burger"), new string("mp_player_int_eat_burger"), new string("Sandwich"), new AnimOptions
            {
                Prop = "prop_sandwich_01",
                PropBone = 18905,
                PropPlacement = new object[] { 0.13, 0.05, 0.02, -50.0, 16.0, 60.0 },
                EmoteMoving = true,
            }),
            ["cola"] = new AnimModel(new string("amb@world_human_drinking@coffee@male@idle_a"), new string("idle_c"), new string("Soda"), new AnimOptions
            {
                Prop = "prop_ecola_can",
                PropBone = 28422,
                PropPlacement = new object[] { 0.0, 0.0, 0.0, 0.0, 0.0, 130.0 },
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["egobar"] = new AnimModel(new string("mp_player_inteat@burger"), new string("mp_player_int_eat_burger"), new string("Ego Bar"), new AnimOptions
            {
                Prop = "prop_choc_ego",
                PropBone = 60309,
                PropPlacement = new object[] { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 },
                EmoteMoving = true,
            }),
            ["wine"] = new AnimModel(new string("anim@heists@humane_labs@finale@keycards"), new string("ped_a_enter_loop"), new string("Wine"), new AnimOptions
            {
                Prop = "prop_drink_redwine",
                PropBone = 18905,
                PropPlacement = new object[] { 0.10, -0.03, 0.03, -100.0, 0.0, -10.0 },
                EmoteMoving = true,
                EmoteLoop = true
            }),
            ["emptyglass"] = new AnimModel(new string("anim@heists@humane_labs@finale@keycards"), new string("ped_a_enter_loop"), new string("Flute"), new AnimOptions
            {
                Prop = "prop_champ_flute",
                PropBone = 18905,
                PropPlacement = new object[] { 0.10, -0.03, 0.03, -100.0, 0.0, -10.0 },
                EmoteMoving = true,
                EmoteLoop = true
            }),
            ["champagne"] = new AnimModel(new string("anim@heists@humane_labs@finale@keycards"), new string("ped_a_enter_loop"), new string("Champagne"), new AnimOptions
            {
                Prop = "prop_drink_champ",
                PropBone = 18905,
                PropPlacement = new object[] { 0.10, -0.03, 0.03, -100.0, 0.0, -10.0 },
                EmoteMoving = true,
                EmoteLoop = true
            }),
            ["smoke4"] = new AnimModel(new string("amb@world_human_smoking@male@male_a@enter"), new string("enter"), new string("Cigar"), new AnimOptions
            {
                Prop = "prop_cigar_02",
                PropBone = 47419,
                PropPlacement = new object[] { 0.010, 0.0, 0.0, 50.0, 0.0, -80.0 },
                EmoteMoving = true,
                EmoteDuration = 2600
            }),
            ["smoke5"] = new AnimModel(new string("amb@world_human_smoking@male@male_a@enter"), new string("enter"), new string("Cigar 2"), new AnimOptions
            {
                Prop = "prop_cigar_01",
                PropBone = 47419,
                PropPlacement = new object[] { 0.010, 0.0, 0.0, 50.0, 0.0, -80.0 },
                EmoteMoving = true,
                EmoteDuration = 2600
            }),
            ["guitar1"] = new AnimModel(new string("amb@world_human_musician@guitar@male@idle_a"), new string("idle_b"), new string("Guitar"), new AnimOptions
            {
                Prop = "prop_acc_guitar_01",
                PropBone = 24818,
                PropPlacement = new object[] { -0.1, 0.31, 0.1, 0.0, 20.0, 150.0 },
                EmoteMoving = true,
                EmoteLoop = true
            }),
            ["guitar2"] = new AnimModel(new string("switch@trevor@guitar_beatdown"), new string("001370_02_trvs_8_guitar_beatdown_idle_busker"), new string("Guitar 2"), new AnimOptions
            {
                Prop = "prop_acc_guitar_01",
                PropBone = 24818,
                PropPlacement = new object[] { -0.05, 0.31, 0.1, 0.0, 20.0, 150.0 },
                EmoteMoving = true,
                EmoteLoop = true
            }),
            ["guitar3"] = new AnimModel(new string("amb@world_human_musician@guitar@male@idle_a"), new string("idle_b"), new string("Guitar Electric"), new AnimOptions
            {
                Prop = "prop_el_guitar_01",
                PropBone = 24818,
                PropPlacement = new object[] { -0.1, 0.31, 0.1, 0.0, 20.0, 150.0 },
                EmoteMoving = true,
                EmoteLoop = true
            }),
            ["guitar4"] = new AnimModel(new string("amb@world_human_musician@guitar@male@idle_a"), new string("idle_b"), new string("Guitar Electric 2"), new AnimOptions
            {
                Prop = "prop_el_guitar_03",
                PropBone = 24818,
                PropPlacement = new object[] { -0.1, 0.31, 0.1, 0.0, 20.0, 150.0 },
                EmoteMoving = true,
                EmoteLoop = true
            }),
            ["readbook"] = new AnimModel(new string("cellphone@"), new string("cellphone_text_read_base"), new string("Book"), new AnimOptions
            {
                Prop = "prop_novel_01",
                PropBone = 6286,
                PropPlacement = new object[] { 0.15, 0.03, -0.065, 0.0, 180.0, 90.0 },
                EmoteMoving = true,
                EmoteLoop = true
            }),
            ["gift1"] = new AnimModel(new string("impexp_int-0"), new string("mp_m_waremech_01_dual-0"), new string("Bouquet"), new AnimOptions
            {
                Prop = "prop_snow_flower_02",
                PropBone = 24817,
                PropPlacement = new object[] { -0.29, 0.40, -0.02, -90.0, -90.0, 0.0 },
                EmoteMoving = true,
                EmoteLoop = true
            }),
            ["gift2"] = new AnimModel(new string("impexp_int-0"), new string("mp_m_waremech_01_dual-0"), new string("Teddy"), new AnimOptions
            {
                Prop = "v_ilev_mr_rasberryclean",
                PropBone = 24817,
                PropPlacement = new object[] { -0.20, 0.46, -0.016, -180.0, -90.0, 0.0 },
                EmoteMoving = true,
                EmoteLoop = true
            }),
            ["backpack"] = new AnimModel(new string("move_p_m_zero_rucksack"), new string("idle"), new string("Backpack"), new AnimOptions
            {
                Prop = "p_michael_backpack_s",
                PropBone = 24818,
                PropPlacement = new object[] { 0.07, -0.11, -0.05, 0.0, 90.0, 175.0 },
                EmoteMoving = true,
                EmoteLoop = true
            }),
            ["checklist"] = new AnimModel(new string("missfam4"), new string("base"), new string("Clipboard"), new AnimOptions
            {
                Prop = "p_amb_clipboard_01",
                PropBone = 36029,
                PropPlacement = new object[] { 0.16, 0.08, 0.1, -130.0, -50.0, 0.0 },
                EmoteMoving = true,
                EmoteLoop = true
            }),
            ["infomap"] = new AnimModel(new string("amb@world_human_tourist_map@male@base"), new string("base"), new string("Map"), new AnimOptions
            {
                Prop = "prop_tourist_map_01",
                PropBone = 28422,
                PropPlacement = new object[] { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 },
                EmoteMoving = true,
                EmoteLoop = true
            }),
            ["homeless"] = new AnimModel(new string("amb@world_human_bum_freeway@male@base"), new string("base"), new string("Beg"), new AnimOptions
            {
                Prop = "prop_beggers_sign_03",
                PropBone = 58868,
                PropPlacement = new object[] { 0.19, 0.18, 0.0, 5.0, 0.0, 40.0 },
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["drug"] = new AnimModel(new string("amb@world_human_smoking@male@male_a@enter"), new string("enter"), new string("Joint"), new AnimOptions
            {
                Prop = "p_cs_joint_02",
                PropBone = 47419,
                PropPlacement = new object[] { 0.015, -0.009, 0.003, 55.0, 0.0, 110.0 },
                EmoteMoving = true,
                EmoteDuration = 2600
            }),
            ["smoke6"] = new AnimModel(new string("amb@world_human_smoking@male@male_a@enter"), new string("enter"), new string("Cig"), new AnimOptions
            {
                Prop = "prop_amb_ciggy_01",
                PropBone = 47419,
                PropPlacement = new object[] { 0.015, -0.009, 0.003, 55.0, 0.0, 110.0 },
                EmoteMoving = true,
                EmoteDuration = 2600
            }),
            ["bag3"] = new AnimModel(new string("missheistdocksprep1hold_cellphone"), new string("static"), new string("Brief 3"), new AnimOptions
            {
                Prop = "prop_ld_case_01",
                PropBone = 57005,
                PropPlacement = new object[] { 0.10, 0.0, 0.0, 0.0, 280.0, 53.0 },
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["tablet1"] = new AnimModel(new string("amb@world_human_tourist_map@male@base"), new string("base"), new string("Tablet"), new AnimOptions
            {
                Prop = "prop_cs_tablet",
                PropBone = 28422,
                PropPlacement = new object[] { 0.0, -0.03, 0.0, 20.0, -90.0, 0.0 },
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["tablet2"] = new AnimModel(new string("amb@code_human_in_bus_passenger_idles@female@tablet@idle_a"), new string("idle_a"), new string("Tablet 2"), new AnimOptions
            {
                Prop = "prop_cs_tablet",
                PropBone = 28422,
                PropPlacement = new object[] { -0.05, 0.0, 0.0, 0.0, 0.0, 0.0 },
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["phone1"] = new AnimModel(new string("cellphone@"), new string("cellphone_call_listen_base"), new string("Phone Call"), new AnimOptions
            {
                Prop = "prop_npc_phone_02",
                PropBone = 28422,
                PropPlacement = new object[] { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 },
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["phone2"] = new AnimModel(new string("cellphone@"), new string("cellphone_text_read_base"), new string("Phone"), new AnimOptions
            {
                Prop = "prop_npc_phone_02",
                PropBone = 28422,
                PropPlacement = new object[] { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 },
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["clean1"] = new AnimModel(new string("timetable@floyd@clean_kitchen@base"), new string("base"), new string("Clean"), new AnimOptions
            {
                Prop = "prop_sponge_01",
                PropBone = 28422,
                PropPlacement = new object[] { 0.0, 0.0, -0.01, 90.0, 0.0, 0.0 },
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["clean2"] = new AnimModel(new string("amb@world_human_maid_clean@"), new string("base"), new string("Clean 2"), new AnimOptions
            {
                Prop = "prop_sponge_01",
                PropBone = 28422,
                PropPlacement = new object[] { 0.0, 0.0, -0.01, 90.0, 0.0, 0.0 },
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["kamera"] = new AnimModel(new string("missfinale_c2mcs_1"), new string("fin_c2_mcs_1_camman"), new string("Kamera"), new AnimOptions
            {
                Prop = "prop_v_cam_01",
                PropBone = 28422,
                PropPlacement = new object[] { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 },
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["mikrofon"] = new AnimModel(new string("missheistdocksprep1hold_cellphone"), new string("hold_cellphone"), new string("Miktoron"), new AnimOptions
            {
                Prop = "p_ing_microphonel_01",
                PropBone = 60309,
                PropPlacement = new object[] { 0.055, 0.05, 0.0, 240.0, 0.0, 0.0 },
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["bmikrofon"] = new AnimModel(new string("missfra1"), new string("mcs2_crew_idle_m_boom"), new string("Kamera"), new AnimOptions
            {
                Prop = "prop_v_bmike_01",
                PropBone = 28422,
                PropPlacement = new object[] { -0.08, 0.0, 0.0, 0.0, 0.0, 0.0 },
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["guitar5"] = new AnimModel(new string("amb@world_human_musician@guitar@male@idle_a"), new string("idle_b"), new string("Guitar Electric"), new AnimOptions
            {
                Prop = "vw_prop_casino_art_guitar_01a",
                PropBone = 24818,
                PropPlacement = new object[] { -0.1, 0.31, 0.1, 0.0, 20.0, 150.0 },
                EmoteMoving = true,
                EmoteLoop = true
            }),
            ["bag4"] = new AnimModel(new string("missheistdocksprep1hold_cellphone"), new string("static"), new string("Suitcase 2"), new AnimOptions
            {
                Prop = "vw_prop_casino_shopping_bag_01a",
                PropBone = 57005,
                PropPlacement = new object[] { 0.284, -0.029, 0.0, 0.0, 265.5, 145.6 },
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["water1"] = new AnimModel(new string("amb@world_human_drinking@coffee@male@idle_a"), new string("idle_c"), new string("Beer"), new AnimOptions
            {
                Prop = "vw_prop_casino_water_bottle_01a",
                PropBone = 28422,
                PropPlacement = new object[] { 0.01, 0.01, -0.133, 0.9, 0.0, 0.0 },
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["water2"] = new AnimModel(new string("amb@world_human_drinking@coffee@male@idle_a"), new string("idle_c"), new string("Beer"), new AnimOptions
            {
                Prop = "prop_energy_drink",
                PropBone = 28422,
                PropPlacement = new object[] { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 },
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["water3"] = new AnimModel(new string("amb@world_human_drinking@coffee@male@idle_a"), new string("idle_c"), new string("Beer"), new AnimOptions
            {
                Prop = "ba_prop_club_water_bottle",
                PropBone = 28422,
                PropPlacement = new object[] { 0.01, 0.01, -0.133, 0.9, 0.0, 0.0 },
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["tray1"] = new AnimModel(new string("impexp_int-0"), new string("mp_m_waremech_01_dual-0"), new string("Bouquet"), new AnimOptions
            {
                Prop = "apa_mp_h_acc_drink_tray_02",
                PropBone = 24817,
                PropPlacement = new object[] { -0.095, 0.532, 0.0, 28.1, 116.3, 245.5 },
                EmoteMoving = true,
                EmoteLoop = true
            }),
            ["tray2"] = new AnimModel(new string("impexp_int-0"), new string("mp_m_waremech_01_dual-0"), new string("Bouquet"), new AnimOptions
            {
                Prop = "vw_prop_vw_ice_bucket_01a",
                PropBone = 24817,
                PropPlacement = new object[] { -0.095, 0.532, 0.0, 28.1, 116.3, 245.5 },
                EmoteMoving = true,
                EmoteLoop = true
            }),
            ["tray3"] = new AnimModel(new string("impexp_int-0"), new string("mp_m_waremech_01_dual-0"), new string("Bouquet"), new AnimOptions
            {
                Prop = "prop_food_bs_tray_02",
                PropBone = 24817,
                PropPlacement = new object[] { -0.054, 0.518, -0.022, 360.0, 89.4, 3.7 },
                EmoteMoving = true,
                EmoteLoop = true
            }),
            ["champ1"] = new AnimModel(new string("anim@heists@humane_labs@finale@keycards"), new string("ped_a_enter_loop"), new string("Rose"), new AnimOptions
            {
                Prop = "ba_prop_battle_champ_01",
                PropBone = 18905,
                PropPlacement = new object[] { 0.114, -0.343, 0.062, 276.1, 168.0, 0.0 },
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["champ2"] = new AnimModel(new string("anim@heists@humane_labs@finale@keycards"), new string("ped_a_enter_loop"), new string("Rose"), new AnimOptions
            {
                Prop = "ba_prop_battle_champ_closed",
                PropBone = 18905,                // 0.057, -0.117, 0.073, 0.0, 108.0, 69.4
                PropPlacement = new object[] { 0.114, -0.134, 0.036, 276.1, 168.0, 0.0 },
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["champ3"] = new AnimModel(new string("anim@heists@humane_labs@finale@keycards"), new string("ped_a_enter_loop"), new string("Rose"), new AnimOptions
            {
                Prop = "ba_prop_battle_champ_closed_02",
                PropBone = 18905,              // 13 -> 25
                PropPlacement = new object[] { 0.114, -0.134, 0.036, 276.1, 168.0, 0.0 },
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["champ4"] = new AnimModel(new string("anim@heists@humane_labs@finale@keycards"), new string("ped_a_enter_loop"), new string("Rose"), new AnimOptions
            {
                Prop = "ba_prop_battle_champ_closed_03",
                PropBone = 18905,
                PropPlacement = new object[] { 0.114, -0.134, 0.036, 276.1, 168.0, 0.0 },
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["champ5"] = new AnimModel(new string("anim@heists@humane_labs@finale@keycards"), new string("ped_a_enter_loop"), new string("Rose"), new AnimOptions
            {
                Prop = "prop_champ_01a",
                PropBone = 18905,
                PropPlacement = new object[] { 0.101, -0.238, 0.049, 276.1, 168.0, 0.0 },
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["champ6"] = new AnimModel(new string("anim@heists@humane_labs@finale@keycards"), new string("ped_a_enter_loop"), new string("Rose"), new AnimOptions
            {
                Prop = "vw_prop_vw_champ_closed",
                PropBone = 18905,
                PropPlacement = new object[] { 0.101, -0.121, 0.036, 276.1, 168.0, 0.0 },
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["champ7"] = new AnimModel(new string("anim@heists@humane_labs@finale@keycards"), new string("ped_a_enter_loop"), new string("Rose"), new AnimOptions
            {
                Prop = "xs_prop_arena_champ_closed",
                PropBone = 18905,
                PropPlacement = new object[] { 0.101, -0.121, 0.036, 276.1, 168.0, 0.0 },
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["bag5"] = new AnimModel(new string("missheistdocksprep1hold_cellphone"), new string("static"), new string("Suitcase 2"), new AnimOptions
            {
                Prop = "ch_prop_toolbox_01a",
                PropBone = 57005,
                PropPlacement = new object[] { 0.25, 0.0, 0.0, 0.0, 260.0, 53.0 },
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["bag6"] = new AnimModel(new string("missheistdocksprep1hold_cellphone"), new string("static"), new string("Suitcase 2"), new AnimOptions
            {
                Prop = "xm_prop_x17_bag_med_01a",
                PropBone = 57005,
                PropPlacement = new object[] { 0.441, 0.023, -0.029, 0.0, 274.9, 70.4 },
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["bag7"] = new AnimModel(new string("missheistdocksprep1hold_cellphone"), new string("static"), new string("Suitcase 2"), new AnimOptions
            {
                Prop = "ba_prop_battle_bag_01a",
                PropBone = 57005,
                PropPlacement = new object[] { 0.441, 0.023, -0.029, 0.0, 274.9, 70.4 },
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["bag8"] = new AnimModel(new string("missheistdocksprep1hold_cellphone"), new string("static"), new string("Suitcase 2"), new AnimOptions
            {
                Prop = "ba_prop_battle_handbag",
                PropBone = 57005,
                PropPlacement = new object[] { 0.18, 0.023, -0.029, 0.0, 274.9, 70.4 },
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["bag9"] = new AnimModel(new string("missheistdocksprep1hold_cellphone"), new string("static"), new string("Suitcase 2"), new AnimOptions
            {
                Prop = "prop_beachbag_01",
                PropBone = 57005,
                PropPlacement = new object[] { 0.441, 0.023, -0.029, 0.0, 274.9, 70.4 },
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["bag10"] = new AnimModel(new string("missheistdocksprep1hold_cellphone"), new string("static"), new string("Suitcase 2"), new AnimOptions
            {
                Prop = "prop_beachbag_02",
                PropBone = 57005,
                PropPlacement = new object[] { 0.441, 0.023, -0.029, 0.0, 274.9, 70.4 },
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["bag11"] = new AnimModel(new string("missheistdocksprep1hold_cellphone"), new string("static"), new string("Suitcase 2"), new AnimOptions
            {
                Prop = "prop_beachbag_03",
                PropBone = 57005,
                PropPlacement = new object[] { 0.441, 0.023, -0.029, 0.0, 274.9, 70.4 },
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["bag12"] = new AnimModel(new string("missheistdocksprep1hold_cellphone"), new string("static"), new string("Suitcase 2"), new AnimOptions
            {
                Prop = "prop_beachbag_06",
                PropBone = 57005,
                PropPlacement = new object[] { 0.441, 0.023, -0.029, 0.0, 274.9, 70.4 },
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["bag13"] = new AnimModel(new string("missheistdocksprep1hold_cellphone"), new string("static"), new string("Suitcase 2"), new AnimOptions
            {
                Prop = "prop_carrier_bag_01",
                PropBone = 57005,
                PropPlacement = new object[] { 0.297, 0.023, 0.062, 0.0, 244.3, 152.7 },
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["bag14"] = new AnimModel(new string("missheistdocksprep1hold_cellphone"), new string("static"), new string("Suitcase 2"), new AnimOptions
            {
                Prop = "prop_cs_shopping_bag",
                PropBone = 57005,
                PropPlacement = new object[] { 0.284, -0.029, 0.062, 0.0, 244.3, 152.7 },
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["bag15"] = new AnimModel(new string("missheistdocksprep1hold_cellphone"), new string("static"), new string("Suitcase 2"), new AnimOptions
            {
                Prop = "prop_ld_handbag_s",
                PropBone = 57005,
                PropPlacement = new object[] { 0.532, 0.023, -0.029, 0.0, 274.9, 70.4 },
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["bag16"] = new AnimModel(new string("missheistdocksprep1hold_cellphone"), new string("static"), new string("Suitcase 2"), new AnimOptions
            {
                Prop = "prop_med_bag_01b",
                PropBone = 57005,
                PropPlacement = new object[] { 0.338, 0.023, -0.029, 0.0, 274.9, 70.4 },
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["bag17"] = new AnimModel(new string("missheistdocksprep1hold_cellphone"), new string("static"), new string("Suitcase 2"), new AnimOptions
            {
                Prop = "prop_shopping_bags02",
                PropBone = 57005,
                PropPlacement = new object[] { 0.127, 0.023, -0.003, 22.3, 215.0, 152.7 },
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["hay"] = new AnimModel(new string("anim@heists@box_carry@"), new string("idle"), new string("Bouquet"), new AnimOptions
            {
                Prop = "bkr_prop_weed_bigbag_01a",
                PropBone = 24817,
                PropPlacement = new object[] { -0.213, 0.454, 0.041, 95.2, 0, 125.2 },
                EmoteMoving = true,
                EmoteLoop = true
            }),
            ["megafon"] = new AnimModel(new string("missheistdocksprep1hold_cellphone"), new string("hold_cellphone"), new string("Miktoron"), new AnimOptions
            {
                Prop = "prop_megaphone_01",
                PropBone = 60309,
                PropPlacement = new object[] { 0.073, 0.041, -0.006, 235.2, 0, 0 },
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["mikrofon2"] = new AnimModel(new string("missheistdocksprep1hold_cellphone"), new string("hold_cellphone"), new string("Miktoron"), new AnimOptions
            {
                Prop = "prop_microphone_02",
                PropBone = 60309,
                PropPlacement = new object[] { 0.073, 0.041, -0.006, 235.2, 0, 0 },
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["umbrella2"] = new AnimModel(new string("amb@world_human_drinking@coffee@male@base"), new string("base"), new string("Umbrella"), new AnimOptions
            {
                Prop = "prop_stickhbird",
                PropBone = 57005,
                PropPlacement = new object[] { 0.089, -0.086, -0.022, 289.4, 43.7, 353.7 },
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["umbrella3"] = new AnimModel(new string("amb@world_human_drinking@coffee@male@base"), new string("base"), new string("Umbrella"), new AnimOptions
            {
                Prop = "prop_stickbfly",
                PropBone = 57005,
                PropPlacement = new object[] { 0.089, -0.086, -0.022, 289.4, 43.7, 353.7 },
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["pankart"] = new AnimModel(new string("anim@heists@humane_labs@finale@keycards"), new string("ped_a_enter_loop"), new string("Rose"), new AnimOptions
            {
                Prop = "prop_cs_protest_sign_01",
                PropBone = 18905,
                PropPlacement = new object[] { 0.121, 0.359, -0.117, 279.4, 110.9, 339.4 },
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["job1"] = new AnimModel(new string("missheistdocksprep1hold_cellphone"), new string("hold_cellphone"), new string("Miktoron"), new AnimOptions
            {
                Prop = "prop_sign_mallet",
                PropBone = 60309,
                PropPlacement = new object[] { 0.073, 0.041, -0.006, 186.6, 98, 128 },
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["job2"] = new AnimModel(new string("missheistdocksprep1hold_cellphone"), new string("hold_cellphone"), new string("Miktoron"), new AnimOptions
            {
                Prop = "prop_tool_consaw",
                PropBone = 60309,
                PropPlacement = new object[] { 0.073, 0.041, -0.006, 185.2, 79.4, 230.9 },
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["job3"] = new AnimModel(new string("missheistdocksprep1hold_cellphone"), new string("hold_cellphone"), new string("Miktoron"), new AnimOptions
            {
                Prop = "prop_tool_drill",
                PropBone = 60309,
                PropPlacement = new object[] { 0.073, 0.041, -0.006, 115.2, 0, 179.4 },
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["job4"] = new AnimModel(new string("missheistdocksprep1hold_cellphone"), new string("hold_cellphone"), new string("Miktoron"), new AnimOptions
            {
                Prop = "prop_tool_pickaxe",
                PropBone = 60309,
                PropPlacement = new object[] { 0.073, 0.041, -0.006, 115.2, 0, 179.4 },
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["job5"] = new AnimModel(new string("missheistdocksprep1hold_cellphone"), new string("hold_cellphone"), new string("Miktoron"), new AnimOptions
            {
                Prop = "prop_tool_mallet",
                PropBone = 60309,
                PropPlacement = new object[] { 0.073, 0.041, -0.006, 115.2, 0, 179.4 },
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["job6"] = new AnimModel(new string("missheistdocksprep1hold_cellphone"), new string("hold_cellphone"), new string("Miktoron"), new AnimOptions
            {
                Prop = "prop_tool_shovel006",
                PropBone = 60309,
                PropPlacement = new object[] { 1.01, 0.787, -0.641, 318, 273.7, 218 },
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["job7"] = new AnimModel(new string("missheistdocksprep1hold_cellphone"), new string("hold_cellphone"), new string("Miktoron"), new AnimOptions
            {
                Prop = "prop_wateringcan",
                PropBone = 60309,
                PropPlacement = new object[] { 0.28, -0.086, -0.006, 50.9, 205.2, 179.4 },
                EmoteLoop = true,
                EmoteMoving = true,
            }),

            ["tool1"] = new AnimModel(new string("anim@amb@clubhouse@bar@drink@idle_a"), new string("idle_a_bartender"), new string("Teddy"), new AnimOptions
            {
                Prop = "prop_handrake",
                PropBone = 24817,
                PropPlacement = new object[] { -0.847, 1.502, 0.089, 345.2, 89.4, 182.3 },
                EmoteMoving = true,
                EmoteLoop = true
            }),
            ["tool2"] = new AnimModel(new string("impexp_int-0"), new string("mp_m_waremech_01_dual-0"), new string("Teddy"), new AnimOptions
            {
                Prop = "imp_prop_car_jack_01a",
                PropBone = 24817,
                PropPlacement = new object[] { -1.244, 1.153, 0.041, 15.2, 90.9, 182.3 },
                EmoteMoving = true,
                EmoteLoop = true
            }),
            ["tool3"] = new AnimModel(new string("impexp_int-0"), new string("mp_m_waremech_01_dual-0"), new string("Teddy"), new AnimOptions
            {
                Prop = "xs_prop_x18_car_jack_01a",
                PropBone = 24817,
                PropPlacement = new object[] { -1.244, 1.153, 0.041, 15.2, 90.9, 182.3 },
                EmoteMoving = true,
                EmoteLoop = true
            }),
            ["bucket"] = new AnimModel(new string("missheistdocksprep1hold_cellphone"), new string("static"), new string("Suitcase 2"), new AnimOptions
            {
                Prop = "vw_prop_vw_ice_bucket_02a", ///
                PropBone = 57005,
                PropPlacement = new object[] { 0.375, -0.054, 0.026, 156.6, 100.9, 130.9 },
                EmoteLoop = true,
                EmoteMoving = true,
            }),

            ["carrybox2"] = new AnimModel(new string("anim@heists@box_carry@"), new string("idle"), new string("Box"), new AnimOptions
            {
                Prop = "prop_cardbordbox_02a",
                PropBone = 60309,
                PropPlacement = new object[] { -0.197, 0.041, 0.391, 102.3, 0, 113.7 },
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["carrybox3"] = new AnimModel(new string("anim@heists@box_carry@"), new string("idle"), new string("Box"), new AnimOptions
            {
                Prop = "prop_cardbordbox_03a",
                PropBone = 60309,
                PropPlacement = new object[] { -0.101, 0.28, 0.454, 82.3, 23.7, 103.7 },
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["carrybox4"] = new AnimModel(new string("anim@heists@box_carry@"), new string("idle"), new string("Box"), new AnimOptions
            {
                Prop = "prop_cardbordbox_05a",
                PropBone = 60309,
                PropPlacement = new object[] { -0.101, 0.28, 0.454, 82.3, 23.7, 103.7 },
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["carrybox5"] = new AnimModel(new string("anim@heists@box_carry@"), new string("idle"), new string("Box"), new AnimOptions
            {
                Prop = "prop_cardbordbox_01a",
                PropBone = 60309,
                PropPlacement = new object[] { 0.137, 0.073, 0.232, 280.9, 9.4, 36.6 },
                EmoteLoop = true,
                EmoteMoving = true,
            }),

            ["beer2"] = new AnimModel(new string("amb@world_human_drinking@coffee@male@idle_a"), new string("idle_c"), new string("Beer"), new AnimOptions
            {
                Prop = "prop_beerdusche",
                PropBone = 28422,
                PropPlacement = new object[] { -0.003, 0.0, -0.121, 0.0, 0.0, 0.0 },
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["beer3"] = new AnimModel(new string("amb@world_human_drinking@coffee@male@idle_a"), new string("idle_c"), new string("Beer"), new AnimOptions
            {
                Prop = "prop_beer_am",
                PropBone = 28422,
                PropPlacement = new object[] { -0.003, 0.0, -0.121, 0.0, 0.0, 0.0 },
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["beer4"] = new AnimModel(new string("amb@world_human_drinking@coffee@male@idle_a"), new string("idle_c"), new string("Beer"), new AnimOptions
            {
                Prop = "prop_beer_bar",
                PropBone = 28422,
                PropPlacement = new object[] { -0.003, 0.0, -0.121, 0.0, 0.0, 0.0 },
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["beer5"] = new AnimModel(new string("amb@world_human_drinking@coffee@male@idle_a"), new string("idle_c"), new string("Beer"), new AnimOptions
            {
                Prop = "prop_beer_blr",
                PropBone = 28422,
                PropPlacement = new object[] { -0.003, 0.0, -0.121, 0.0, 0.0, 0.0 },
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["beer6"] = new AnimModel(new string("amb@world_human_drinking@coffee@male@idle_a"), new string("idle_c"), new string("Beer"), new AnimOptions
            {
                Prop = "prop_beer_jakey",
                PropBone = 28422,
                PropPlacement = new object[] { -0.003, 0.0, -0.121, 0.0, 0.0, 0.0 },
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["beer7"] = new AnimModel(new string("amb@world_human_drinking@coffee@male@idle_a"), new string("idle_c"), new string("Beer"), new AnimOptions
            {
                Prop = "prop_beer_logger",
                PropBone = 28422,
                PropPlacement = new object[] { -0.003, 0.0, -0.121, 0.0, 0.0, 0.0 },
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["beer8"] = new AnimModel(new string("amb@world_human_drinking@coffee@male@idle_a"), new string("idle_c"), new string("Beer"), new AnimOptions
            {
                Prop = "prop_beer_patriot",
                PropBone = 28422,
                PropPlacement = new object[] { -0.003, 0.0, -0.121, 0.0, 0.0, 0.0 },
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["beer9"] = new AnimModel(new string("amb@world_human_drinking@coffee@male@idle_a"), new string("idle_c"), new string("Beer"), new AnimOptions
            {
                Prop = "prop_beer_pissh",
                PropBone = 28422,
                PropPlacement = new object[] { -0.003, 0.0, -0.121, 0.0, 0.0, 0.0 },
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["beer10"] = new AnimModel(new string("amb@world_human_drinking@coffee@male@idle_a"), new string("idle_c"), new string("Beer"), new AnimOptions
            {
                Prop = "prop_beer_pride",
                PropBone = 28422,
                PropPlacement = new object[] { -0.003, 0.0, -0.121, 0.0, 0.0, 0.0 },
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["beer11"] = new AnimModel(new string("amb@world_human_drinking@coffee@male@idle_a"), new string("idle_c"), new string("Beer"), new AnimOptions
            {
                Prop = "prop_beer_stz",
                PropBone = 28422,
                PropPlacement = new object[] { -0.003, 0.0, -0.121, 0.0, 0.0, 0.0 },
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["beer12"] = new AnimModel(new string("amb@world_human_drinking@coffee@male@idle_a"), new string("idle_c"), new string("Beer"), new AnimOptions
            {
                Prop = "prop_cs_beer_bot_02",
                PropBone = 28422,
                PropPlacement = new object[] { -0.003, 0.0, -0.003, 0.0, 0.0, 0.0 },
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["beer13"] = new AnimModel(new string("amb@world_human_drinking@coffee@male@idle_a"), new string("idle_c"), new string("Beer"), new AnimOptions
            {
                Prop = "prop_cs_beer_bot_40oz",
                PropBone = 28422,
                PropPlacement = new object[] { -0.003, 0.0, -0.042, 0.0, 0.0, 0.0 },
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["beer14"] = new AnimModel(new string("amb@world_human_drinking@coffee@male@idle_a"), new string("idle_c"), new string("Beer"), new AnimOptions
            {
                Prop = "prop_cs_whiskey_bottle",
                PropBone = 28422,
                PropPlacement = new object[] { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 },
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["beer15"] = new AnimModel(new string("amb@world_human_drinking@coffee@male@idle_a"), new string("idle_c"), new string("Beer"), new AnimOptions
            {
                Prop = "ba_prop_battle_whiskey_opaque_s",
                PropBone = 28422,
                PropPlacement = new object[] { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 },
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["beer16"] = new AnimModel(new string("amb@world_human_drinking@coffee@male@idle_a"), new string("idle_c"), new string("Beer"), new AnimOptions
            {
                Prop = "prop_whiskey_bottle",
                PropBone = 28422,
                PropPlacement = new object[] { 0.10, 0.0, 0.0, 0.0, 0.0, 0.0 },
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["beer17"] = new AnimModel(new string("amb@world_human_drinking@coffee@male@idle_a"), new string("idle_c"), new string("Beer"), new AnimOptions
            {
                Prop = "p_whiskey_notop",
                PropBone = 28422,
                PropPlacement = new object[] { 0.10, 0.0, 0.0, 0.0, 0.0, 0.0 },
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["dildooooxlelor;)"] = new AnimModel(new string("missheistdocksprep1hold_cellphone"), new string("static"), new string("Beer"), new AnimOptions
            {
                Prop = "prop_cs_dildo_01",
                PropBone = 23553,
                PropPlacement = new object[] { -0.068, 0.18, 0.0, 279.6, 0.0, 345.4 },
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["sand1"] = new AnimModel(new string("x"), new string("static"), new string("Beer"), new AnimOptions
            {
                Prop = "apa_mp_h_din_chair_12",
                PropBone = 11816,
                PropPlacement = new object[] { 0.435, -0.427, -0.049, 45, 270.8, 175.6 },
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["sand2"] = new AnimModel(new string("x"), new string("static"), new string("Beer"), new AnimOptions
            {
                Prop = "apa_mp_h_din_chair_09",
                PropBone = 11816,
                PropPlacement = new object[] { 0.435, -0.427, -0.049, 45, 270.8, 175.6 },
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["sand3"] = new AnimModel(new string("x"), new string("static"), new string("Beer"), new AnimOptions
            {
                Prop = "apa_mp_h_stn_chairarm_11",
                PropBone = 11816,
                PropPlacement = new object[] { 0.435, -0.427, -0.049, 45, 270.8, 175.6 },
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["sand4"] = new AnimModel(new string("x"), new string("static"), new string("Beer"), new AnimOptions
            {
                Prop = "bkr_prop_clubhouse_chair_01",
                PropBone = 11816,
                PropPlacement = new object[] { 0.435, -0.427, -0.049, 45, 270.8, 175.6 },
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["sand5"] = new AnimModel(new string("x"), new string("static"), new string("Beer"), new AnimOptions
            {
                Prop = "bkr_prop_weed_chair_01a",
                PropBone = 11816,
                PropPlacement = new object[] { 0.435, -0.427, -0.049, 45, 270.8, 175.6 },
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["sand6"] = new AnimModel(new string("x"), new string("static"), new string("Beer"), new AnimOptions
            {
                Prop = "gr_prop_gr_chair02_ped",
                PropBone = 11816,
                PropPlacement = new object[] { 0.435, -0.427, -0.049, 45, 270.8, 175.6 },
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["sand7"] = new AnimModel(new string("x"), new string("static"), new string("Beer"), new AnimOptions
            {
                Prop = "hei_heist_din_chair_01",
                PropBone = 11816,
                PropPlacement = new object[] { 0.435, -0.427, -0.049, 45, 270.8, 175.6 },
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["sand8"] = new AnimModel(new string("x"), new string("static"), new string("Beer"), new AnimOptions
            {
                Prop = "hei_heist_din_chair_03",
                PropBone = 11816,
                PropPlacement = new object[] { 0.435, -0.427, -0.049, 45, 270.8, 175.6 },
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["sand8"] = new AnimModel(new string("x"), new string("static"), new string("Beer"), new AnimOptions
            {
                Prop = "hei_heist_din_chair_05",
                PropBone = 11816,
                PropPlacement = new object[] { 0.435, -0.427, -0.049, 45, 270.8, 175.6 },
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["garbage"] = new AnimModel(new string("anim@heists@narcotics@trash"), new string("walk"), new string("trash"), new AnimOptions
            {
                Prop = "hei_prop_heist_binbag",
                PropBone = 57005,
                PropPlacement = new object[] { 0.275, -0.054, 0.026, 156.6, 100.9, 130.9 }, // PropPlacement = new object[] { 0.375, -0.054, 0.026, 156.6, 100.9, 130.9 },
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["acitbox"] = new AnimModel(new string("anim@heists@box_carry@"), new string("idle"), new string("Box"), new AnimOptions
            {
                Prop = "prop_barrel_exp_01c",
                PropBone = 60309,
                PropPlacement = new object[] { -0.101, 0.28, 0.454, 82.3, 23.7, 103.7 },
                EmoteLoop = true,
                EmoteMoving = true,
            }),
            ["yeredus"] = new AnimModel(new string("dagdoll@human"), new string("electrocute"), new string("Box"), new AnimOptions
            {
                EmoteLoop = false,
                EmoteMoving = false,
            }),
        };


        [Command("anim", aliases: new string[] { "a" })]
        public static void PlayerAnimation(PlayerModel p, params string[] args)
        {
            if (args.Length <= 0) { return; }
            if (p.CurrentWeapon != 2725352035) { p.CurrentWeapon = 2725352035; }
            if (p.Vehicle != null) { return; }
            if (p.jailTime > 0 || p.adminJail > 0) { MainChat.SendErrorChat(p, "[错误] 无法在监狱期间使用."); return; }
            
            if (p.HasStreamSyncedMetaData("AttachedObjects"))
            {
                AttachmentSystem.deleteAllAttachs(p);
            }

            AnimModel anim = Anims.Where(x => x.Key == args[0].ToString()).FirstOrDefault().Value;
            if (anim == null) { MainChat.SendErrorChat(p, "[错误] 无效动作!"); return; }
            int flag = 0;
            int duration = -1;
            if (anim.AnimOptions != null)
            {
                if (anim.AnimOptions.EmoteLoop == true) { flag = 1; }
                if (anim.AnimOptions.EmoteMoving == true) { flag = 53; }

                /*if (anim.AnimOptions.Prop != null)
                {
                    AttachObj p1 = new AttachObj()
                    {
                        attach = p.Id,
                        bone = anim.AnimOptions.PropBone,
                        off = new Vector3(float.Parse(anim.AnimOptions.PropPlacement[0].ToString()), float.Parse(anim.AnimOptions.PropPlacement[1].ToString()), float.Parse(anim.AnimOptions.PropPlacement[2].ToString())),
                        rot = new Vector3(float.Parse(anim.AnimOptions.PropPlacement[3].ToString()), float.Parse(anim.AnimOptions.PropPlacement[4].ToString()), float.Parse(anim.AnimOptions.PropPlacement[5].ToString()))
                    };
                    LProp x = PropStreamer.Create(anim.AnimOptions.Prop, p.Position, p.Rotation, attach: p1, dimension: p.Dimension);
                    p.lscSetData(EntityData.PlayerEntityData.propInfo, x);
                }
                
                if (anim.AnimOptions.SecondProp != null)
                {                    
                    AttachObj p2 = new AttachObj()
                    {
                        attach = p.Id,
                        bone = anim.AnimOptions.SecondPropBone,
                        off = new Vector3(float.Parse(anim.AnimOptions.SecondPropPlacement[0].ToString()), float.Parse(anim.AnimOptions.SecondPropPlacement[1].ToString()), float.Parse(anim.AnimOptions.SecondPropPlacement[2].ToString())),
                        rot = new Vector3(float.Parse(anim.AnimOptions.SecondPropPlacement[3].ToString()), float.Parse(anim.AnimOptions.SecondPropPlacement[4].ToString()), float.Parse(anim.AnimOptions.SecondPropPlacement[5].ToString()))
                    };
                    await Task.Delay(500);

                    if (!p.Exists)
                        return;

                    LProp y = PropStreamer.Create(anim.AnimOptions.SecondProp, p.Position, p.Rotation, attach: p2, dimension: p.Dimension);
                    p.lscSetData(EntityData.PlayerEntityData.secondPropInfo, y);
                }*/

                if (anim.AnimOptions.Prop != null)
                {
                    AttachmentSystem.AddAttach(p, new AttachmentSystem.ObjectModel()
                    {
                        Model = anim.AnimOptions.Prop,
                        boneIndex = anim.AnimOptions.PropBone.ToString(),
                        xPos = float.Parse(anim.AnimOptions.PropPlacement[0].ToString()),
                        yPos = float.Parse(anim.AnimOptions.PropPlacement[1].ToString()),
                        zPos = float.Parse(anim.AnimOptions.PropPlacement[2].ToString()),
                        xRot = float.Parse(anim.AnimOptions.PropPlacement[3].ToString()),
                        yRot = float.Parse(anim.AnimOptions.PropPlacement[4].ToString()),
                        zRot = float.Parse(anim.AnimOptions.PropPlacement[5].ToString())
                    });
                }

                if (anim.AnimOptions.SecondProp != null)
                {
                    AttachmentSystem.AddAttach(p, new AttachmentSystem.ObjectModel()
                    {
                        Model = anim.AnimOptions.SecondProp,
                        boneIndex = anim.AnimOptions.SecondPropBone.ToString(),
                        xPos = float.Parse(anim.AnimOptions.SecondPropPlacement[0].ToString()),
                        yPos = float.Parse(anim.AnimOptions.SecondPropPlacement[1].ToString()),
                        zPos = float.Parse(anim.AnimOptions.SecondPropPlacement[2].ToString()),
                        xRot = float.Parse(anim.AnimOptions.SecondPropPlacement[3].ToString()),
                        yRot = float.Parse(anim.AnimOptions.SecondPropPlacement[4].ToString()),
                        zRot = float.Parse(anim.AnimOptions.SecondPropPlacement[5].ToString())
                    });
                }
                //p.lscSetData(EntityData.PlayerEntityData.hasProp, true);
                if (anim.AnimOptions.EmoteDuration != null) { duration = (int)anim.AnimOptions.EmoteDuration; }
            }

            if (anim.dict.Length >= 5)
            {
                GlobalEvents.PlayAnimation(p, new string[] { anim.dict, anim.anim }, flag, duration);
            }
            return;
        }

        public static void PlayerServerAnimation(PlayerModel p, params string[] args)
        {
            if (args.Length <= 0) { return; }

            //PlayerStopAnimation(p);
            /*if (p.lscGetdata<bool>(EntityData.PlayerEntityData.hasProp) == true)
            {
                LProp x = p.lscGetdata<LProp>(EntityData.PlayerEntityData.propInfo);
                if (x != null) { x.Delete(); p.DeleteData(EntityData.PlayerEntityData.propInfo); }
                LProp y = p.lscGetdata<LProp>(EntityData.PlayerEntityData.secondPropInfo);
                if (y != null) { y.Delete(); p.DeleteData(EntityData.PlayerEntityData.secondPropInfo); }
                p.DeleteData(EntityData.PlayerEntityData.hasProp);
            }*/
            AnimModel anim = Anims.Where(x => x.Key == args[0].ToString()).FirstOrDefault().Value;
            if (anim == null) { return; }
            int flag = 0;
            int duration = -1;
            if (anim.AnimOptions != null)
            {
                if (anim.AnimOptions.EmoteLoop == true) { flag = 1; }
                if (anim.AnimOptions.EmoteMoving == true) { flag = 53; }

                /*if (anim.AnimOptions.Prop != null)
                {
                    AttachObj p1 = new AttachObj()
                    {
                        attach = p.Id,
                        bone = anim.AnimOptions.PropBone,
                        off = new Vector3(float.Parse(anim.AnimOptions.PropPlacement[0].ToString()), float.Parse(anim.AnimOptions.PropPlacement[1].ToString()), float.Parse(anim.AnimOptions.PropPlacement[2].ToString())),
                        rot = new Vector3(float.Parse(anim.AnimOptions.PropPlacement[3].ToString()), float.Parse(anim.AnimOptions.PropPlacement[4].ToString()), float.Parse(anim.AnimOptions.PropPlacement[5].ToString()))
                    };
                    LProp x = PropStreamer.Create(anim.AnimOptions.Prop, p.Position, p.Rotation, attach: p1, dimension: p.Dimension);
                    p.lscSetData(EntityData.PlayerEntityData.propInfo, x);
                }

                if (anim.AnimOptions.SecondProp != null)
                {
                    AttachObj p2 = new AttachObj()
                    {
                        attach = p.Id,
                        bone = anim.AnimOptions.SecondPropBone,
                        off = new Vector3(float.Parse(anim.AnimOptions.SecondPropPlacement[0].ToString()), float.Parse(anim.AnimOptions.SecondPropPlacement[1].ToString()), float.Parse(anim.AnimOptions.SecondPropPlacement[2].ToString())),
                        rot = new Vector3(float.Parse(anim.AnimOptions.SecondPropPlacement[3].ToString()), float.Parse(anim.AnimOptions.SecondPropPlacement[4].ToString()), float.Parse(anim.AnimOptions.SecondPropPlacement[5].ToString()))
                    };
                    await Task.Delay(500);

                    if (!p.Exists)
                        return;

                    LProp y = PropStreamer.Create(anim.AnimOptions.SecondProp, p.Position, p.Rotation, attach: p2, dimension: p.Dimension);
                    p.lscSetData(EntityData.PlayerEntityData.secondPropInfo, y);
                }

                p.lscSetData(EntityData.PlayerEntityData.hasProp, true);*/
                if (anim.AnimOptions.EmoteDuration != null) { duration = (int)anim.AnimOptions.EmoteDuration; }
            }

            if (anim.dict.Length >= 5)
            {
                GlobalEvents.PlayAnimation(p, new string[] { anim.dict, anim.anim }, flag, duration);
            }
            return;
        }
        [Command("sa")]
        public static void PlayerStopAnimation(PlayerModel p)
        {
            if (p.Vehicle != null) { return; }
            //Delete Datas
            /*if (p.lscGetdata<bool>(EntityData.PlayerEntityData.hasProp) == true)
            {
                LProp x = p.lscGetdata<LProp>(EntityData.PlayerEntityData.propInfo);
                if (x != null) { x.Delete(); p.DeleteData(EntityData.PlayerEntityData.propInfo); }
                LProp y = p.lscGetdata<LProp>(EntityData.PlayerEntityData.secondPropInfo);
                if(y != null) { y.Delete(); p.DeleteData(EntityData.PlayerEntityData.secondPropInfo); }
                p.DeleteData(EntityData.PlayerEntityData.hasProp);
            }*/

            AttachmentSystem.deleteAllAttachs(p);

            if (p.HasData("inFishing"))
                p.DeleteData("inFishing");

            GlobalEvents.StopAnimation(p);
            return;
        }

        [Command("sc", aliases: new string[] { "scenario" })]
        public void PlayerPlayScenario(PlayerModel p, params string[] args)
        {
            if (args.Length <= 0)
            {
                MainChat.SendInfoChat(p, "[用法] /sc [名称]<br>可用选项:<br>cofe / 咖啡 | smoke / 抽烟 | telescope / 望远镜 | homeless(1-3) / 乞丐 | wash / 清洗 | lean / 倚靠 | park / 停车 | clap / 鼓掌 | drill / DRILL | plean / 警察依靠 / drink(1 - 3) / 喝 | phone(1 - 3) / 电话 | gar(1 - 2) / 园丁 | golf / 高尔夫 | ham / 锤子 | chat(1 - 2) / 聊天 | stat / 雕像 | mop / 抹布 / lean(1 - 2) / 倚靠 | clean / 打扫 | sport(1 - 5) / 运动 | music / 音乐 | camera / 相机 | stance(1 - 7) / 姿态 | cigar(1 - 5) / 抽烟 / ray / RAY | reach1 / 到达 | docu(1 - 3) / 文档 | source /资源 | yoga /瑜伽 | atm / ATM | bbq / 烧烤 | cop / 警察 | dg(1 - 3) / 单杠");
                //MainChat.SendInfoChat(p, "[用法] /sc [名称]<br>可用选项:<br>cofe | smoke | telescope | homeless(1-3) | wash | lean | park | clap | drill | plean / drink(1 - 3) | phone(1 - 3) | bahcivan(1 - 2) | golf | cekic | sohbet(1 - 2) | heykel | supurge / yaslan(1 - 2) | temizlik | spor(1 - 5) | muzik | kamera | durus(1 - 7) | sigara(1 - 5) / isin | uzan1 | belge(1 - 3) | kaynak | yoga | atm | barbeku | copkaristir | barfiks(1 - 3)");
                return;
            }
            if (p.jailTime > 0 || p.adminJail > 0) { MainChat.SendErrorChat(p, "[错误] 无法在监狱期间使用."); return; }
            if (p.Vehicle != null) { return; }
            string sc = "WORLD_HUMAN_AA_COFFEE";
            switch (args[0])
            {
                case "cofe":
                    sc = "WORLD_HUMAN_AA_COFFEE";
                    break;

                case "smoke":
                    sc = "WORLD_HUMAN_AA_SMOKE";
                    break;

                case "telescope":
                    sc = "WORLD_HUMAN_BINOCULARS";
                    break;

                case "homeless1":
                    sc = "WORLD_HUMAN_BUM_FREEWAY";
                    break;

                case "homeless2":
                    sc = "WORLD_HUMAN_BUM_SLUMPED";
                    break;

                case "homeless3":
                    sc = "WORLD_HUMAN_BUM_STANDING";
                    break;

                case "wash":
                    sc = "WORLD_HUMAN_BUM_WASH";
                    break;

                case "lean":
                    sc = "WORLD_HUMAN_VALET";
                    break;

                case "park":
                    sc = "WORLD_HUMAN_CAR_PARK_ATTENDANT";
                    break;

                case "clap":
                    sc = "WORLD_HUMAN_CHEERING";
                    break;

                case "docu1":
                    sc = "WORLD_HUMAN_CLIPBOARD";
                    break;

                case "docu2":
                    sc = "WORLD_HUMAN_CLIPBOARD_FACILITY";
                    break;

                case "drill":
                    sc = "WORLD_HUMAN_CONST_DRILL";
                    break;

                case "plean":
                    sc = "WORLD_HUMAN_COP_IDLES";
                    break;

                case "drink1":
                    sc = "WORLD_HUMAN_DRINKING";
                    break;

                case "drink2":
                    sc = "WORLD_HUMAN_DRINKING_FACILITY";
                    break;

                case "drink3":
                    sc = "WORLD_HUMAN_DRINKING_CASINO_TERRACE";
                    break;

                case "cigar1":
                    sc = "WORLD_HUMAN_DRUG_DEALER";
                    break;

                case "stance1":
                    sc = "WORLD_HUMAN_DRUG_DEALER_HARD";
                    break;

                case "phone1":
                    sc = "WORLD_HUMAN_MOBILE_FILM_SHOCKING";
                    break;

                case "gar1":
                    sc = "WORLD_HUMAN_GARDENER_LEAF_BLOWER";
                    break;

                case "gar2":
                    sc = "WORLD_HUMAN_GARDENER_PLANT";
                    break;

                case "stance2":
                    sc = "WORLD_HUMAN_GUARD_STAND";
                    break;

                case "stance3":
                    sc = "WORLD_HUMAN_GUARD_STAND_CASINO";
                    break;

                case "ham":
                    sc = "WORLD_HUMAN_HAMMERING";
                    break;

                case "chat1":
                    sc = "WORLD_HUMAN_HANG_OUT_STREET";
                    break;

                case "chat2":
                    sc = "WORLD_HUMAN_HANG_OUT_STREET_CLUBHOUSE";
                    break;

                case "stat":
                    sc = "WORLD_HUMAN_HUMAN_STATUE";
                    break;

                case "mop":
                    sc = "WORLD_HUMAN_JANITOR";
                    break;

                case "sport1":
                    sc = "WORLD_HUMAN_JOG";
                    break;

                case "sport2":
                    sc = "WORLD_HUMAN_JOG_STANDING";
                    break;

                case "lean1":
                    sc = "WORLD_HUMAN_LEANING";
                    break;

                case "lean2":
                    sc = "WORLD_HUMAN_LEANING_CASINO_TERRACE";
                    break;

                case "clean":
                    sc = "WORLD_HUMAN_MAID_CLEAN";
                    break;

                case "sport3":
                    sc = "WORLD_HUMAN_MUSCLE_FLEX";
                    break;

                case "sport4":
                    sc = "WORLD_HUMAN_MUSCLE_FREE_WEIGHTS";
                    break;

                case "music":
                    sc = "WORLD_HUMAN_MUSICIAN";
                    break;

                case "camera":
                    sc = "WORLD_HUMAN_PAPARAZZI";
                    break;

                case "party":
                    sc = "WORLD_HUMAN_PARTYING";
                    break;

                case "stance4":
                    sc = "WORLD_HUMAN_PROSTITUTE_HIGH_CLASS";
                    break;

                case "stance5":
                    sc = "WORLD_HUMAN_PROSTITUTE_LOW_CLASS";
                    break;

                case "sport5":
                    sc = "WORLD_HUMAN_PUSH_UPS";
                    break;

                case "cigar2":
                    sc = "WORLD_HUMAN_SMOKING";
                    break;

                case "cigar3":
                    sc = "WORLD_HUMAN_SMOKING_CLUBHOUSE";
                    break;

                case "cigar4":
                    sc = "WORLD_HUMAN_SMOKING_POT";
                    break;

                case "cigar5":
                    sc = "WORLD_HUMAN_SMOKING_POT_CLUBHOUSE";
                    break;

                case "ray":
                    sc = "WORLD_HUMAN_STAND_FIRE";
                    break;

                case "phone2":
                    sc = "WORLD_HUMAN_STAND_MOBILE";
                    break;

                case "stance6":
                    sc = "WORLD_HUMAN_STRIP_WATCH_STAND";
                    break;

                case "reach1":
                    sc = "WORLD_HUMAN_SUNBATHE";
                    break;

                case "docu3":
                    sc = "WORLD_HUMAN_TOURIST_MAP";
                    break;

                case "phone3":
                    sc = "WORLD_HUMAN_TOURIST_MOBILE";
                    break;

                case "source":
                    sc = "WORLD_HUMAN_WELDING";
                    break;

                case "stance7":
                    sc = "WORLD_HUMAN_WINDOW_SHOP_BROWSE";
                    break;

                case "yoga":
                    sc = "WORLD_HUMAN_YOGA";
                    break;

                case "atm":
                    sc = "PROP_HUMAN_ATM";
                    break;

                case "bbq":
                    sc = "PROP_HUMAN_BBQ";
                    break;

                case "cop":
                    sc = "PROP_HUMAN_BUM_BIN";
                    break;

                case "dg1":
                    sc = "PROP_HUMAN_MUSCLE_CHIN_UPS";
                    break;

                case "dg2":
                    sc = "PROP_HUMAN_MUSCLE_CHIN_UPS_ARMY";
                    break;

                case "dg3":
                    sc = "PROP_HUMAN_MUSCLE_CHIN_UPS_PRISON";
                    break;

                default:
                    MainChat.SendInfoChat(p, "[用法] /sc [名称]<br>可用选项:<br>cofe / 咖啡 | smoke / 抽烟 | telescope / 望远镜 | homeless(1-3) / 乞丐 | wash / 清洗 | lean / 倚靠 | park / 停车 | clap / 鼓掌 | drill / DRILL | plean / 警察依靠 / drink(1 - 3) / 喝 | phone(1 - 3) / 电话 | gar(1 - 2) / 园丁 | golf / 高尔夫 | ham / 锤子 | chat(1 - 2) / 聊天 | stat / 雕像 | mop / 抹布 / lean(1 - 2) / 倚靠 | clean / 打扫 | sport(1 - 5) / 运动 | music / 音乐 | camera / 相机 | stance(1 - 7) / 姿态 | cigar(1 - 5) / 抽烟 / ray / RAY | reach1 / 到达 | docu(1 - 3) / 文档 | source /资源 | yoga /瑜伽 | atm / ATM | bbq / 烧烤 | cop / 警察 | dg(1 - 3) / 单杠");
                    //
                    p.SetData("inFishing", true);
                    p.EmitAsync("PlayerScenario", sc);
                    break;
            }
        }
    }
}

