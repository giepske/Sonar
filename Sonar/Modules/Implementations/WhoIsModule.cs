using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Sonar.Logging;

namespace Sonar.Modules.Implementations
{
    public class WhoIsModule : WebServerModule
    {
        private readonly string _host;
        private readonly string _hostExtension;
        public override string Name { get; set; } = "WhoIs Module";

        //list of whois servers for each host extension
        private Dictionary<string, string> _whoisServers = new Dictionary<string, string>(new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("ac", "whois.nic.ac"),
            new KeyValuePair<string, string>("ad", "whois.ripe.net"),
            new KeyValuePair<string, string>("ae", "whois.aeda.net.ae"),
            new KeyValuePair<string, string>("aero", "whois.aero"),
            new KeyValuePair<string, string>("af", "whois.nic.af"),
            new KeyValuePair<string, string>("ag", "whois.nic.ag"),
            new KeyValuePair<string, string>("ai", "whois.ai"),
            new KeyValuePair<string, string>("al", "whois.ripe.net"),
            new KeyValuePair<string, string>("am", "whois.amnic.net"),
            new KeyValuePair<string, string>("as", "whois.nic.as"),
            new KeyValuePair<string, string>("asia", "whois.nic.asia"),
            new KeyValuePair<string, string>("at", "whois.nic.at"),
            new KeyValuePair<string, string>("au", "whois.aunic.net"),
            new KeyValuePair<string, string>("aw", "whois.nic.aw"),
            new KeyValuePair<string, string>("ax", "whois.ax "),
            new KeyValuePair<string, string>("az", "whois.ripe.net"),
            new KeyValuePair<string, string>("ba", "whois.ripe.net"),
            new KeyValuePair<string, string>("bar", "whois.nic.bar"),
            new KeyValuePair<string, string>("be", "whois.dns.be"),
            new KeyValuePair<string, string>("berlin", "whois.nic.berlin"),
            new KeyValuePair<string, string>("best", "whois.nic.best"),
            new KeyValuePair<string, string>("bg", "whois.register.bg"),
            new KeyValuePair<string, string>("bi", "whois.nic.bi"),
            new KeyValuePair<string, string>("biz", "whois.neulevel.biz"),
            new KeyValuePair<string, string>("bj", "www.nic.bj"),
            new KeyValuePair<string, string>("bo", "whois.nic.bo"),
            new KeyValuePair<string, string>("br", "whois.nic.br"),
            new KeyValuePair<string, string>("br.com", "whois.centralnic.com"),
            new KeyValuePair<string, string>("bt", "whois.netnames.net"),
            new KeyValuePair<string, string>("bw", "whois.nic.net.bw"),
            new KeyValuePair<string, string>("by", "whois.cctld.by"),
            new KeyValuePair<string, string>("bz", "whois.belizenic.bz"),
            new KeyValuePair<string, string>("bzh", "whois-bzh.nic.fr"),
            new KeyValuePair<string, string>("ca", "whois.cira.ca"),
            new KeyValuePair<string, string>("cat", "whois.cat"),
            new KeyValuePair<string, string>("cc", "whois.nic.cc"),
            new KeyValuePair<string, string>("cd", "whois.nic.cd"),
            new KeyValuePair<string, string>("ceo", "whois.nic.ceo"),
            new KeyValuePair<string, string>("cf", "whois.dot.cf"),
            new KeyValuePair<string, string>("ch", "whois.nic.ch "),
            new KeyValuePair<string, string>("ci", "whois.nic.ci"),
            new KeyValuePair<string, string>("ck", "whois.nic.ck"),
            new KeyValuePair<string, string>("cl", "whois.nic.cl"),
            new KeyValuePair<string, string>("cloud", "whois.nic.cloud"),
            new KeyValuePair<string, string>("club", "whois.nic.club"),
            new KeyValuePair<string, string>("cn", "whois.cnnic.net.cn"),
            new KeyValuePair<string, string>("cn.com", "whois.centralnic.com"),
            new KeyValuePair<string, string>("co", "whois.nic.co"),
            new KeyValuePair<string, string>("co.nl", "whois.co.nl"),
            new KeyValuePair<string, string>("com", "whois.verisign-grs.com"),
            new KeyValuePair<string, string>("coop", "whois.nic.coop"),
            new KeyValuePair<string, string>("cx", "whois.nic.cx"),
            new KeyValuePair<string, string>("cy", "whois.ripe.net"),
            new KeyValuePair<string, string>("cz", "whois.nic.cz"),
            new KeyValuePair<string, string>("de", "whois.denic.de"),
            new KeyValuePair<string, string>("dk", "whois.dk-hostmaster.dk"),
            new KeyValuePair<string, string>("dm", "whois.nic.cx"),
            new KeyValuePair<string, string>("dz", "whois.nic.dz"),
            new KeyValuePair<string, string>("ec", "whois.nic.ec"),
            new KeyValuePair<string, string>("edu", "whois.educause.net"),
            new KeyValuePair<string, string>("ee", "whois.tld.ee"),
            new KeyValuePair<string, string>("eg", "whois.ripe.net"),
            new KeyValuePair<string, string>("es", "whois.nic.es"),
            new KeyValuePair<string, string>("eu", "whois.eu"),
            new KeyValuePair<string, string>("eu.com", "whois.centralnic.com"),
            new KeyValuePair<string, string>("eus", "whois.nic.eus"),
            new KeyValuePair<string, string>("fi", "whois.fi"),
            new KeyValuePair<string, string>("fo", "whois.nic.fo"),
            new KeyValuePair<string, string>("fr", "whois.nic.fr"),
            new KeyValuePair<string, string>("gb", "whois.ripe.net"),
            new KeyValuePair<string, string>("gb.com", "whois.centralnic.com"),
            new KeyValuePair<string, string>("gb.net", "whois.centralnic.com"),
            new KeyValuePair<string, string>("qc.com", "whois.centralnic.com"),
            new KeyValuePair<string, string>("ge", "whois.ripe.net"),
            new KeyValuePair<string, string>("gg", "whois.gg"),
            new KeyValuePair<string, string>("gi", "whois2.afilias-grs.net"),
            new KeyValuePair<string, string>("gl", "whois.nic.gl"),
            new KeyValuePair<string, string>("gm", "whois.ripe.net"),
            new KeyValuePair<string, string>("gov", "whois.nic.gov"),
            new KeyValuePair<string, string>("gr", "whois.ripe.net"),
            new KeyValuePair<string, string>("gs", "whois.nic.gs"),
            new KeyValuePair<string, string>("gy", "whois.registry.gy"),
            new KeyValuePair<string, string>("hamburg", "whois.nic.hamburg"),
            new KeyValuePair<string, string>("hiphop", "whois.uniregistry.net"),
            new KeyValuePair<string, string>("hk", "whois.hknic.net.hk"),
            new KeyValuePair<string, string>("hm", "whois.registry.hm"),
            new KeyValuePair<string, string>("hn", "whois2.afilias-grs.net"),
            new KeyValuePair<string, string>("host", "whois.nic.host"),
            new KeyValuePair<string, string>("hr", "whois.dns.hr"),
            new KeyValuePair<string, string>("ht", "whois.nic.ht"),
            new KeyValuePair<string, string>("hu", "whois.nic.hu"),
            new KeyValuePair<string, string>("hu.com", "whois.centralnic.com"),
            new KeyValuePair<string, string>("id", "whois.pandi.or.id"),
            new KeyValuePair<string, string>("ie", "whois.domainregistry.ie"),
            new KeyValuePair<string, string>("il", "whois.isoc.org.il"),
            new KeyValuePair<string, string>("im", "whois.nic.im"),
            new KeyValuePair<string, string>("in", "whois.inregistry.net"),
            new KeyValuePair<string, string>("info", "whois.afilias.info"),
            new KeyValuePair<string, string>("ing", "domain-registry-whois.l.google.com"),
            new KeyValuePair<string, string>("ink", "whois.centralnic.com"),
            new KeyValuePair<string, string>("int", "whois.isi.edu"),
            new KeyValuePair<string, string>("io", "whois.nic.io"),
            new KeyValuePair<string, string>("iq", "whois.cmc.iq"),
            new KeyValuePair<string, string>("ir", "whois.nic.ir"),
            new KeyValuePair<string, string>("is", "whois.isnic.is"),
            new KeyValuePair<string, string>("it", "whois.nic.it"),
            new KeyValuePair<string, string>("je", "whois.je"),
            new KeyValuePair<string, string>("jobs", "jobswhois.verisign-grs.com"),
            new KeyValuePair<string, string>("jp", "whois.jprs.jp"),
            new KeyValuePair<string, string>("ke", "whois.kenic.or.ke"),
            new KeyValuePair<string, string>("kg", "whois.domain.kg"),
            new KeyValuePair<string, string>("ki", "whois.nic.ki"),
            new KeyValuePair<string, string>("kr", "whois.kr"),
            new KeyValuePair<string, string>("kz", "whois.nic.kz"),
            new KeyValuePair<string, string>("la", "whois2.afilias-grs.net"),
            new KeyValuePair<string, string>("li", "whois.nic.li"),
            new KeyValuePair<string, string>("london", "whois.nic.london"),
            new KeyValuePair<string, string>("lt", "whois.domreg.lt"),
            new KeyValuePair<string, string>("lu", "whois.restena.lu"),
            new KeyValuePair<string, string>("lv", "whois.nic.lv"),
            new KeyValuePair<string, string>("ly", "whois.lydomains.com"),
            new KeyValuePair<string, string>("ma", "whois.iam.net.ma"),
            new KeyValuePair<string, string>("mc", "whois.ripe.net"),
            new KeyValuePair<string, string>("md", "whois.nic.md"),
            new KeyValuePair<string, string>("me", "whois.nic.me"),
            new KeyValuePair<string, string>("mg", "whois.nic.mg"),
            new KeyValuePair<string, string>("mil", "whois.nic.mil"),
            new KeyValuePair<string, string>("mk", "whois.ripe.net"),
            new KeyValuePair<string, string>("ml", "whois.dot.ml"),
            new KeyValuePair<string, string>("mo", "whois.monic.mo"),
            new KeyValuePair<string, string>("mobi", "whois.dotmobiregistry.net"),
            new KeyValuePair<string, string>("ms", "whois.nic.ms"),
            new KeyValuePair<string, string>("mt", "whois.ripe.net"),
            new KeyValuePair<string, string>("mu", "whois.nic.mu"),
            new KeyValuePair<string, string>("museum", "whois.museum"),
            new KeyValuePair<string, string>("mx", "whois.nic.mx"),
            new KeyValuePair<string, string>("my", "whois.mynic.net.my"),
            new KeyValuePair<string, string>("mz", "whois.nic.mz"),
            new KeyValuePair<string, string>("na", "whois.na-nic.com.na"),
            new KeyValuePair<string, string>("name", "whois.nic.name"),
            new KeyValuePair<string, string>("nc", "whois.nc"),
            new KeyValuePair<string, string>("net", "whois.verisign-grs.com"),
            new KeyValuePair<string, string>("nf", "whois.nic.cx"),
            new KeyValuePair<string, string>("ng", "whois.nic.net.ng"),
            new KeyValuePair<string, string>("nl", "whois.domain-registry.nl"),
            new KeyValuePair<string, string>("no", "whois.norid.no"),
            new KeyValuePair<string, string>("no.com", "whois.centralnic.com"),
            new KeyValuePair<string, string>("nu", "whois.nic.nu"),
            new KeyValuePair<string, string>("nz", "whois.srs.net.nz"),
            new KeyValuePair<string, string>("om", "whois.registry.om"),
            new KeyValuePair<string, string>("ong", "whois.publicinterestregistry.net"),
            new KeyValuePair<string, string>("ooo", "whois.nic.ooo"),
            new KeyValuePair<string, string>("org", "whois.pir.org"),
            new KeyValuePair<string, string>("paris", "whois-paris.nic.fr"),
            new KeyValuePair<string, string>("pe", "kero.yachay.pe"),
            new KeyValuePair<string, string>("pf", "whois.registry.pf"),
            new KeyValuePair<string, string>("pics", "whois.uniregistry.net"),
            new KeyValuePair<string, string>("pl", "whois.dns.pl"),
            new KeyValuePair<string, string>("pm", "whois.nic.pm"),
            new KeyValuePair<string, string>("pr", "whois.nic.pr"),
            new KeyValuePair<string, string>("press", "whois.nic.press"),
            new KeyValuePair<string, string>("pro", "whois.registrypro.pro"),
            new KeyValuePair<string, string>("pt", "whois.dns.pt"),
            new KeyValuePair<string, string>("pub", "whois.unitedtld.com"),
            new KeyValuePair<string, string>("pw", "whois.nic.pw"),
            new KeyValuePair<string, string>("qa", "whois.registry.qa"),
            new KeyValuePair<string, string>("re", "whois.nic.re"),
            new KeyValuePair<string, string>("ro", "whois.rotld.ro"),
            new KeyValuePair<string, string>("rs", "whois.rnids.rs"),
            new KeyValuePair<string, string>("ru", "whois.tcinet.ru"),
            new KeyValuePair<string, string>("sa", "saudinic.net.sa"),
            new KeyValuePair<string, string>("sa.com", "whois.centralnic.com"),
            new KeyValuePair<string, string>("sb", "whois.nic.net.sb"),
            new KeyValuePair<string, string>("sc", "whois2.afilias-grs.net"),
            new KeyValuePair<string, string>("se", "whois.nic-se.se"),
            new KeyValuePair<string, string>("se.com", "whois.centralnic.com"),
            new KeyValuePair<string, string>("se.net", "whois.centralnic.com"),
            new KeyValuePair<string, string>("sg", "whois.nic.net.sg"),
            new KeyValuePair<string, string>("sh", "whois.nic.sh"),
            new KeyValuePair<string, string>("si", "whois.arnes.si"),
            new KeyValuePair<string, string>("sk", "whois.sk-nic.sk"),
            new KeyValuePair<string, string>("sm", "whois.nic.sm"),
            new KeyValuePair<string, string>("st", "whois.nic.st"),
            new KeyValuePair<string, string>("so", "whois.nic.so"),
            new KeyValuePair<string, string>("su", "whois.tcinet.ru"),
            new KeyValuePair<string, string>("sx", "whois.sx"),
            new KeyValuePair<string, string>("sy", "whois.tld.sy"),
            new KeyValuePair<string, string>("tc", "whois.adamsnames.tc"),
            new KeyValuePair<string, string>("tel", "whois.nic.tel"),
            new KeyValuePair<string, string>("tf", "whois.nic.tf"),
            new KeyValuePair<string, string>("th", "whois.thnic.net"),
            new KeyValuePair<string, string>("tj", "whois.nic.tj"),
            new KeyValuePair<string, string>("tk", "whois.nic.tk"),
            new KeyValuePair<string, string>("tl", "whois.domains.tl"),
            new KeyValuePair<string, string>("tm", "whois.nic.tm"),
            new KeyValuePair<string, string>("tn", "whois.ati.tn"),
            new KeyValuePair<string, string>("to", "whois.tonic.to"),
            new KeyValuePair<string, string>("top", "whois.nic.top"),
            new KeyValuePair<string, string>("tp", "whois.domains.tl"),
            new KeyValuePair<string, string>("tr", "whois.nic.tr"),
            new KeyValuePair<string, string>("travel", "whois.nic.travel"),
            new KeyValuePair<string, string>("tw", "whois.twnic.net.tw"),
            new KeyValuePair<string, string>("tv", "whois.nic.tv"),
            new KeyValuePair<string, string>("tz", "whois.tznic.or.tz"),
            new KeyValuePair<string, string>("ua", "whois.ua"),
            new KeyValuePair<string, string>("ug", "whois.co.ug"),
            new KeyValuePair<string, string>("uk", "whois.nic.uk"),
            new KeyValuePair<string, string>("uk.com", "whois.centralnic.com"),
            new KeyValuePair<string, string>("uk.net", "whois.centralnic.com"),
            new KeyValuePair<string, string>("ac.uk", "whois.ja.net"),
            new KeyValuePair<string, string>("gov.uk", "whois.ja.net"),
            new KeyValuePair<string, string>("us", "whois.nic.us"),
            new KeyValuePair<string, string>("us.com", "whois.centralnic.com"),
            new KeyValuePair<string, string>("uy", "nic.uy"),
            new KeyValuePair<string, string>("uy.com", "whois.centralnic.com"),
            new KeyValuePair<string, string>("uz", "whois.cctld.uz"),
            new KeyValuePair<string, string>("va", "whois.ripe.net"),
            new KeyValuePair<string, string>("vc", "whois2.afilias-grs.net"),
            new KeyValuePair<string, string>("ve", "whois.nic.ve"),
            new KeyValuePair<string, string>("vg", "ccwhois.ksregistry.net"),
            new KeyValuePair<string, string>("vu", "vunic.vu"),
            new KeyValuePair<string, string>("wang", "whois.nic.wang"),
            new KeyValuePair<string, string>("wf", "whois.nic.wf"),
            new KeyValuePair<string, string>("wiki", "whois.nic.wiki"),
            new KeyValuePair<string, string>("ws", "whois.website.ws"),
            new KeyValuePair<string, string>("xxx", "whois.nic.xxx"),
            new KeyValuePair<string, string>("xyz", "whois.nic.xyz"),
            new KeyValuePair<string, string>("yu", "whois.ripe.net"),
            new KeyValuePair<string, string>("za.com", "whois.centralnic.com")
        });

        public WhoIsModule(string host) : base(host)
        {
            _host = ParseHost(host);
            _hostExtension = ParseHostExtension(host);
        }

        private string ParseHost(string host)
        {
            var uri = new Uri(host);

            return uri.Host;
        }

        private string ParseHostExtension(string host)
        {
            //remove slashes at the end of the host if any
            while (host.EndsWith("/"))
                host = host.Substring(0, host.Length - 1);

            //most of the time we can easily parse the host extension
            if (host.Count(c => c == '.') == 1)
                return host.Substring(host.IndexOf(".") + 1);

            //if not we try to parse until we find a match in the whois server list
            string temp = host;
            while (temp.Contains("."))
            {
                temp = host.Substring(host.IndexOf("."));

                if (_whoisServers.Keys.Any(extension => extension == temp))
                    return temp;
            }

            //if no match is found we return null for later
            return null;
        }

        public override Task<ModuleResult> Execute(Data data)
        {
            if(!string.IsNullOrEmpty(_hostExtension))
                Logger.Log(LogType.Info, $"Getting whois information for host: " + _host);
            else
            {
                Logger.Log(LogType.Error, $"Unable to find whois server for host (is your host extension invalid?): " + _host);

                return Task.FromResult(ModuleResult.Create(this, ResultType.Error,
                    "Unable to find whois server for host (is your host extension invalid?): " + _host));
            }

            if (TryGetWhoisInfo(out string whoIs))
            {
                return Task.FromResult(ModuleResult.Create(this, ResultType.Warning, 
                    $"WhoIs information has been found, check if it contains personal information and if so, contact your domain provider:\n\r\n\r{whoIs}"));
            }

            return Task.FromResult(ModuleResult.Create(this, ResultType.Error, $"No whois information could be found for host: " + _host));
        }

        private bool TryGetWhoisInfo(out string whoIs)
        {
            whoIs = "";

            var whoisServer = _whoisServers.First(e => e.Key == _hostExtension);

            using (TcpClient tcpClient = new TcpClient())
            {
                try
                {
                    if (tcpClient.ConnectAsync(whoisServer.Value, 43).Wait(2000))
                    {
                        using (var stream = tcpClient.GetStream())
                        {
                            byte[] buffer = Encoding.UTF8.GetBytes(_host + Environment.NewLine);

                            stream.Write(buffer, 0, buffer.Length);

                            buffer = new byte[1024];

                            int bytesRead;
                            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                whoIs += Encoding.UTF8.GetString(buffer, 0, bytesRead) + "\r\n";
                            }

                            //return true if no match value is set and let the user find out
                            if(!string.IsNullOrEmpty(whoIs))
                                return true;
                        }
                    }
                }
                catch (Exception)
                {
                    // ignored
                }
            }

            whoIs = null;
            return false;
        }

    }
}