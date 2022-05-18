using System.ComponentModel.DataAnnotations;

namespace Spam.Enums;

public enum PressureType
{
    [Display(Name = Common.Strings.Spam.BasePressure)]
    Base = 1,

    [Display(Name = Common.Strings.Spam.ImagePressure)]
    Image = 2,

    [Display(Name = Common.Strings.Spam.LengthPressure)]
    Length = 3,

    [Display(Name = Common.Strings.Spam.LinePressure)]
    Line = 4,

    [Display(Name = Common.Strings.Spam.PingPressure)]
    Ping = 5,

    [Display(Name = Common.Strings.Spam.RepeatPressure)]
    Repeat = 6
}