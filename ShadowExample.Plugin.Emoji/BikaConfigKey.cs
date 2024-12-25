using ShadowPluginLoader.MetaAttributes;

namespace ShadowExample.Plugin.Emoji;

[ShadowPluginSettingClass(typeof(EmojiPlugin), "Setting")]
[ShadowSettingClass("ShadowExample.Plugin.Emoji","EmojiSetting")]
public enum BikaConfigKey
{
    [ShadowSetting(typeof(int), "1", "Api分流")]
    ApiShunt,

    [ShadowSetting(typeof(bool), "true", "登陆后记住我")]
    RememberMe,

    [ShadowSetting(typeof(string), "tt",comment: "测试名称")]
    TestName
}