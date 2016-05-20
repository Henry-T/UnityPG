using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace ConfigEditor
{
    public class EnumerationExtension : MarkupExtension
    {
        private Type enumType;

        public EnumerationExtension(Type enumType)
        {
            if (enumType == null)
                throw new ArgumentNullException("enumType");

            if (!enumType.IsEnum)
                throw new ArgumentException("enumType must be an enum");

            this.enumType = enumType;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Enum.GetNames(enumType);
        }
    }
}
