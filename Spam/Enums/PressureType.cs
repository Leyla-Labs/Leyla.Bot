using System.ComponentModel.DataAnnotations;

namespace Spam.Enums;

public enum PressureType
{
    [Display(Name = Db.Strings.Spam.BasePressure)]
    Base = 1,

    [Display(Name = Db.Strings.Spam.ImagePressure)]
    Image = 2,

    [Display(Name = Db.Strings.Spam.LengthPressure)]
    Length = 3,

    [Display(Name = Db.Strings.Spam.LinePressure)]
    Line = 4,

    [Display(Name = Db.Strings.Spam.PingPressure)]
    Ping = 5,

    [Display(Name = Db.Strings.Spam.RepeatPressure)]
    Repeat = 6
}