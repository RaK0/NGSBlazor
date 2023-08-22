using NGSBlazor.Client.Interfaces;

namespace NGSBlazor.Client.LocalItems
{
    public class RememberMeLocalItem : ILocalItem
    {
        public string Name => "RememberMe";
        public bool? Value { get; set; }        
    }
}
