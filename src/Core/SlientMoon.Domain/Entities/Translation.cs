using SlientMoon.Domain.Common;
using SlientMoon.Domain.Enums;

namespace SlientMoon.Domain.Entities
{
    public class Translation : BaseEntity
    {
        public string Key { get; set; }
        public LanguageCode LanguageCode { get; set; }
        public string Value { get; set; }
    }
}
