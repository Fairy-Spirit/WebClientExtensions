# WebClientExtensions
**A WebClient extension class that effectively solves the problem of timeout retry by C#.**
## Example：
*string url = "https://raw.githubusercontent.com/Fairy-Spirit/WebClientExtensions/master/README.md";<br/>
CancellationTokenSource cts = new CancellationTokenSource();
<br/>
**1) Download String**<br/>
string result = "";<br/>
using (MyWebClient client = new MyWebClient(1000,3))<br/>
{  <br/>
   &ensp;&ensp;&ensp;&ensp;client.UseDefaultCredentials = true;<br/>
   &ensp;&ensp;&ensp;&ensp;client.Headers.Add("user-agent", @"Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; Trident/6.0)");<br/>
   &ensp;&ensp;&ensp;&ensp;result = await client.DownloadStringAsync(url, cts.Token, Encoding.UTF8);<br/>
}
<br/>
**2) Download Bytes**<br/>
byte[] bytes = null;<br/>
using (MyWebClient client = new MyWebClient(1000,3))<br/>
{<br/>
   &ensp;&ensp;&ensp;&ensp;client.UseDefaultCredentials = true;<br/>
   &ensp;&ensp;&ensp;&ensp;client.Headers.Add("user-agent", @"Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; Trident/6.0)");<br/>
   &ensp;&ensp;&ensp;&ensp;bytes = await client.DownloadBytesAsync(url, cts.Token);<br/>
}*
