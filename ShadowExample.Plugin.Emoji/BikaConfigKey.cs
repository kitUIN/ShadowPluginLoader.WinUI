using ShadowPluginLoader.MetaAttributes;

namespace ShadowExample.Plugin.Emoji;

[ShadowPluginSettingClass("EmojiPlugin","ShadowExample.Plugin.Emoji")]
public enum BikaConfigKey
{
    [ShadowSetting("int","1","Api分流")]
    ApiShunt,
    [ShadowSetting("bool","true","登陆后记住我")]
    RememberMe
}