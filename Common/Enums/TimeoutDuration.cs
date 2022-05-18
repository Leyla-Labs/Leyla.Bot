using System.ComponentModel.DataAnnotations;
using Common.Strings;

namespace Common.Enums;

public enum TimeoutDuration
{
    [Display(Name = Spam.NoTimeout)] None = 0,
    [Display(Name = Spam.SixtySeconds)] SixtySeconds = 1,
    [Display(Name = Spam.FiveMinutes)] FiveMinutes = 2,
    [Display(Name = Spam.TenMinutes)] TenMinutes = 3,
    [Display(Name = Spam.OneHour)] OneHour = 4,
    [Display(Name = Spam.OneDay)] OneDay = 5,
    [Display(Name = Spam.OneWeek)] OneWeek = 6
}