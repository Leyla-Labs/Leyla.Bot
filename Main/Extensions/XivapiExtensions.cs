using Main.Helper;
using SixLabors.ImageSharp;
using xivapi_cs.Enums;
using Attribute = xivapi_cs.Enums.Attribute;

namespace Main.Extensions;

internal static class XivapiExtensions
{
    /// <summary>
    ///     Loads Grand Company crest (if necessary) and returns it.
    /// </summary>
    /// <param name="gc">Grand Company to get crest for.</param>
    /// <returns>ImageSharp Image, Grand Company crest.</returns>
    public static async Task<Image> GetCrestAsync(this GrandCompany gc)
    {
        var img = gc switch
        {
            GrandCompany.Maelstrom => Enums.CharacterSheet.Image.GcMaelstrom,
            GrandCompany.OrderTwinAdder => Enums.CharacterSheet.Image.GcTwinAdder,
            GrandCompany.ImmortalFlames => Enums.CharacterSheet.Image.GcImmortalFlames,
            _ => throw new ArgumentOutOfRangeException(nameof(gc), gc, "Grand Company not in resources.")
        };

        return (await ResourceHelper.Instance).GetImage(img);
    }

    /// <summary>
    ///     Returns primary and secondary attributes for jobs.
    /// </summary>
    /// <remarks>
    ///     Crafters and gatherers do not have any primary attributes, so only secondary ones are returned for them.
    /// </remarks>
    /// <param name="job">The job to get attributes for.</param>
    /// <returns>A collection of attributes, primary then secondary.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Job not implemented.</exception>
    public static ICollection<Attribute> GetDisplayAttributes(this Job job)
    {
        switch (job)
        {
            case Job.Pld:
            case Job.War:
            case Job.Drk:
            case Job.Gnb:
                return new[]
                {
                    Attribute.Strength,
                    Attribute.Vitality,
                    Attribute.CriticalHit,
                    Attribute.Determination,
                    Attribute.Tenacity
                };
            case Job.Whm:
            case Job.Sch:
            case Job.Ast:
            case Job.Sge:
                return new[]
                {
                    Attribute.Mind,
                    Attribute.Vitality,
                    Attribute.CriticalHit,
                    Attribute.DirectHitRate,
                    Attribute.Determination
                };
            case Job.Mnk:
            case Job.Drg:
            case Job.Nin:
            case Job.Sam:
            case Job.Rpr:
                return new[]
                {
                    Attribute.Strength,
                    Attribute.Vitality,
                    Attribute.CriticalHit,
                    Attribute.Determination,
                    Attribute.DirectHitRate
                };
            case Job.Brd:
            case Job.Mch:
            case Job.Dnc:
                return new[]
                {
                    Attribute.Dexterity,
                    Attribute.Vitality,
                    Attribute.CriticalHit,
                    Attribute.Determination,
                    Attribute.DirectHitRate
                };
            case Job.Blm:
            case Job.Smn:
            case Job.Rdm:
            case Job.Blu:
                return new[]
                {
                    Attribute.Intelligence,
                    Attribute.Vitality,
                    Attribute.CriticalHit,
                    Attribute.Determination,
                    Attribute.DirectHitRate
                };
            case Job.Crp:
            case Job.Bsm:
            case Job.Arm:
            case Job.Gsm:
            case Job.Ltw:
            case Job.Wvr:
            case Job.Alc:
            case Job.Cul:
                return new[]
                {
                    Attribute.Craftmanship,
                    Attribute.Control,
                    Attribute.Cp
                };
            case Job.Min:
            case Job.Btn:
            case Job.Fsh:
                return new[]
                {
                    Attribute.Gathering,
                    Attribute.Perception,
                    Attribute.Gp
                };
            default:
                throw new ArgumentOutOfRangeException(nameof(job), job, "Job not implemented");
        }
    }
}