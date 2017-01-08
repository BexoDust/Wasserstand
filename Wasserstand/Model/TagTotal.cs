    using WpfHelper;

namespace Wasserstand.Model
{
    public class TagTotal :ANotifyBase
    {


        public string Name
        {
            get { return Get(() => Name); }
            set { Set(value, () => Name); }
        }


        public double Value
        {
            get { return Get(() => Value); }
            set { Set(value, () => Value); }
        }
    }
}
