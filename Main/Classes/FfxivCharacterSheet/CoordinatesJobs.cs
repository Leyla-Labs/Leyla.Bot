using System.Numerics;
using xivapi_cs.Enums;

namespace Main.Classes.FfxivCharacterSheet;

public static class JobCoordinates
{
    private static int DistNormal => 56;
    private static int DistCategory => 84;
    private static int DistVertical => 100;

    public static Vector2 Get(Job job)
    {
        return job switch
        {
            Job.Pld => new Vector2(575, 632),
            Job.War => new Vector2(Get(Job.Pld).X + DistNormal, Get(Job.Pld).Y),
            Job.Drk => new Vector2(Get(Job.War).X + DistNormal, Get(Job.Pld).Y),
            Job.Gnb => new Vector2(Get(Job.Drk).X + DistNormal, Get(Job.Pld).Y),
            Job.Whm => new Vector2(Get(Job.Gnb).X + DistCategory, Get(Job.Pld).Y),
            Job.Sch => new Vector2(Get(Job.Whm).X + DistNormal, Get(Job.Pld).Y),
            Job.Ast => new Vector2(Get(Job.Sch).X + DistNormal, Get(Job.Pld).Y),
            Job.Sge => new Vector2(Get(Job.Ast).X + DistNormal, Get(Job.Pld).Y),
            Job.Mnk => new Vector2(Get(Job.Pld).X, Get(Job.Pld).Y + DistVertical),
            Job.Drg => new Vector2(Get(Job.Mnk).X + DistNormal, Get(Job.Mnk).Y),
            Job.Nin => new Vector2(Get(Job.Drg).X + DistNormal, Get(Job.Mnk).Y),
            Job.Sam => new Vector2(Get(Job.Nin).X + DistNormal, Get(Job.Mnk).Y),
            Job.Rpr => new Vector2(Get(Job.Sam).X + DistNormal, Get(Job.Mnk).Y),
            Job.Brd => new Vector2(Get(Job.Rpr).X + DistCategory, Get(Job.Mnk).Y),
            Job.Mch => new Vector2(Get(Job.Brd).X + DistNormal, Get(Job.Mnk).Y),
            Job.Dnc => new Vector2(Get(Job.Mch).X + DistNormal, Get(Job.Mnk).Y),
            Job.Blm => new Vector2(Get(Job.Dnc).X + DistNormal, Get(Job.Mnk).Y),
            Job.Smn => new Vector2(Get(Job.Sge).X + DistCategory, Get(Job.Pld).Y),
            Job.Rdm => new Vector2(Get(Job.Smn).X + DistNormal, Get(Job.Pld).Y),
            Job.Blu => new Vector2(Get(Job.Blm).X + DistCategory, Get(Job.Mnk).Y),
            Job.Crp => new Vector2(Get(Job.Pld).X, Get(Job.Mnk).Y + DistVertical),
            Job.Bsm => new Vector2(Get(Job.Crp).X + DistNormal, Get(Job.Crp).Y),
            Job.Arm => new Vector2(Get(Job.Bsm).X + DistNormal, Get(Job.Crp).Y),
            Job.Gsm => new Vector2(Get(Job.Arm).X + DistNormal, Get(Job.Crp).Y),
            Job.Ltw => new Vector2(Get(Job.Gsm).X + DistNormal, Get(Job.Crp).Y),
            Job.Wvr => new Vector2(Get(Job.Ltw).X + DistNormal, Get(Job.Crp).Y),
            Job.Alc => new Vector2(Get(Job.Wvr).X + DistNormal, Get(Job.Crp).Y),
            Job.Cul => new Vector2(Get(Job.Alc).X + DistNormal, Get(Job.Crp).Y),
            Job.Min => new Vector2(Get(Job.Cul).X + DistNormal, Get(Job.Crp).Y),
            Job.Btn => new Vector2(Get(Job.Min).X + DistNormal, Get(Job.Crp).Y),
            Job.Fsh => new Vector2(Get(Job.Btn).X + DistNormal, Get(Job.Crp).Y),
            _ => throw new ArgumentOutOfRangeException(nameof(job), job, null)
        };
    }
}