namespace Kachuwa.Security
{
    public class ScriptPolicyBuilder:BasePolicyBuilder
    {
        public ScriptPolicyBuilder() : base("script-src")
        {
        }

       
    }
    public class DefaultPolicyBuilder : BasePolicyBuilder
    {
        public DefaultPolicyBuilder() : base("default-src")
        {
        }
    }
    public class StylePolicyBuilder : BasePolicyBuilder
    {
        public StylePolicyBuilder() : base("style-src")
        {
        }
    }
    public class MediaPolicyBuilder : BasePolicyBuilder
    {
        public MediaPolicyBuilder() : base("media-src")
        {
        }
    }
    public class FramePolicyBuilder : BasePolicyBuilder
    {
        public FramePolicyBuilder() : base("child-src")
        {
        }
    }
    public class ImagePolicyBuilder : BasePolicyBuilder
    {
        public ImagePolicyBuilder() : base("img-src")
        {
        }
    }
    public class ConnectPolicyBuilder : BasePolicyBuilder
    {
        public ConnectPolicyBuilder() : base("connect-src")
        {
        }
    }
    public class BaseUriolicyBuilder : BasePolicyBuilder
    {
        public BaseUriolicyBuilder() : base("script-src")
        {
        }
    }
    public class FormActionPolicyBuilder : BasePolicyBuilder
    {
        public FormActionPolicyBuilder() : base("form-action")
        {
        }
    }
    public class ObjectPolicyBuilder : BasePolicyBuilder
    {
        public ObjectPolicyBuilder() : base("object-src")
        {
        }
    }
}