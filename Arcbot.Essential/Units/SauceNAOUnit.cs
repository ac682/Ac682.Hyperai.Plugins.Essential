using Hyperai.Units;
using System.Linq;
using Hyperai.Units.Attributes;
using Hyperai.Events;
using HyperaiShell.Foundation.Authorization.Attributes;
using Hyperai.Messages;
using Hyperai.Relations;
using Hyperai.Messages.ConcreteModels;
using System.IO;
using System;
using HyperaiShell.Foundation.ModelExtensions;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Arcbot.Essential.Units
{
    public class SauceNAOUnit : UnitBase
    {

        [Receive(MessageEventType.Group)]
        [Extract("*!sauce", true)]
        [Description("用 SauceNAO 搜索图片出处")]
        public async Task GetSauce(Group group, MessageChain raw, Member sender)
        {
            ImageBase img = null;
            if(raw.Any(x => x is Quote))
            {
                var source = await raw.OfMessageRepliedByAsync();
                img = (ImageBase)source.First(x => x is ImageBase);
            }

            if (img != null)
            {
                await group.SendPlainAsync("在找了在找了😚");
            }else
            {
                await group.SendPlainAsync("木有图片😥");
                return;
            }
            try
            {

                using MemoryStream writer = new MemoryStream();
                using (var reader = img.OpenRead())
                {
                    await reader.CopyToAsync(writer);
                    writer.Position = 0;
                }
                var client = new HttpClient
                {
                    BaseAddress = new Uri("https://saucenao.com/")
                };
                var content = new MultipartFormDataContent
                {
                    { new StreamContent(writer), "file", img.Url.AbsoluteUri }
                };
                var response = await client.PostAsync("search.php?output_type=2&numres=1", content);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var builder = raw.CanBeReplied() ? raw.MakeReply() : new MessageChainBuilder();
                    builder.Add(new At(sender.Identity));
                    builder.AddPlain("来啦来啦😘");
                    var obj = JsonConvert.DeserializeObject<JObject>(json);
                    foreach (var result in obj.Value<JArray>("results"))
                    {
                        var thumbnail = result["header"].Value<string>("thumbnail");
                        var similarity = result["header"].Value<string>("similarity");
                        var url = result["data"]["ext_urls"]?.Values<string>()?.FirstOrDefault() ?? "[NOURL]";
                        var title = result["data"].Value<string>("title") ?? "[UNKNOWN]";
                        var member = result["data"].Value<string>("member_name") ?? "[UNKNOWN]";
                        builder.AddImage(new Uri(thumbnail));
                        builder.AddPlain($"([{similarity}%]({title} - {member}: {url})\n");
                    }
                    var msg = builder.Build();
                    await group.SendAsync(msg);
                }
            }
            catch
            {
                await group.SendPlainAsync($"出错了出错了😫");
            }
        }
    }
}