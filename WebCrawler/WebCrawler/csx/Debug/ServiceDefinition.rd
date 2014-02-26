<?xml version="1.0" encoding="utf-8"?>
<serviceModel xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" name="WebCrawler" generation="1" functional="0" release="0" Id="29b81c52-eedf-445d-b072-1e83d3f73fcd" dslVersion="1.2.0.0" xmlns="http://schemas.microsoft.com/dsltools/RDSM">
  <groups>
    <group name="WebCrawlerGroup" generation="1" functional="0" release="0">
      <componentports>
        <inPort name="Dashboard:Endpoint1" protocol="http">
          <inToChannel>
            <lBChannelMoniker name="/WebCrawler/WebCrawlerGroup/LB:Dashboard:Endpoint1" />
          </inToChannel>
        </inPort>
      </componentports>
      <settings>
        <aCS name="Crawler:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/WebCrawler/WebCrawlerGroup/MapCrawler:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </maps>
        </aCS>
        <aCS name="CrawlerInstances" defaultValue="[1,1,1]">
          <maps>
            <mapMoniker name="/WebCrawler/WebCrawlerGroup/MapCrawlerInstances" />
          </maps>
        </aCS>
        <aCS name="Dashboard:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/WebCrawler/WebCrawlerGroup/MapDashboard:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </maps>
        </aCS>
        <aCS name="DashboardInstances" defaultValue="[1,1,1]">
          <maps>
            <mapMoniker name="/WebCrawler/WebCrawlerGroup/MapDashboardInstances" />
          </maps>
        </aCS>
      </settings>
      <channels>
        <lBChannel name="LB:Dashboard:Endpoint1">
          <toPorts>
            <inPortMoniker name="/WebCrawler/WebCrawlerGroup/Dashboard/Endpoint1" />
          </toPorts>
        </lBChannel>
      </channels>
      <maps>
        <map name="MapCrawler:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/WebCrawler/WebCrawlerGroup/Crawler/Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </setting>
        </map>
        <map name="MapCrawlerInstances" kind="Identity">
          <setting>
            <sCSPolicyIDMoniker name="/WebCrawler/WebCrawlerGroup/CrawlerInstances" />
          </setting>
        </map>
        <map name="MapDashboard:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/WebCrawler/WebCrawlerGroup/Dashboard/Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </setting>
        </map>
        <map name="MapDashboardInstances" kind="Identity">
          <setting>
            <sCSPolicyIDMoniker name="/WebCrawler/WebCrawlerGroup/DashboardInstances" />
          </setting>
        </map>
      </maps>
      <components>
        <groupHascomponents>
          <role name="Crawler" generation="1" functional="0" release="0" software="C:\Users\Kevin\Documents\Visual Studio 2012\Projects\WebCrawler\WebCrawler\csx\Debug\roles\Crawler" entryPoint="base\x64\WaHostBootstrapper.exe" parameters="base\x64\WaWorkerHost.exe " memIndex="1792" hostingEnvironment="consoleroleadmin" hostingEnvironmentVersion="2">
            <settings>
              <aCS name="Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="" />
              <aCS name="__ModelData" defaultValue="&lt;m role=&quot;Crawler&quot; xmlns=&quot;urn:azure:m:v1&quot;&gt;&lt;r name=&quot;Crawler&quot; /&gt;&lt;r name=&quot;Dashboard&quot;&gt;&lt;e name=&quot;Endpoint1&quot; /&gt;&lt;/r&gt;&lt;/m&gt;" />
            </settings>
            <resourcereferences>
              <resourceReference name="DiagnosticStore" defaultAmount="[4096,4096,4096]" defaultSticky="true" kind="Directory" />
              <resourceReference name="EventStore" defaultAmount="[1000,1000,1000]" defaultSticky="false" kind="LogStore" />
            </resourcereferences>
          </role>
          <sCSPolicy>
            <sCSPolicyIDMoniker name="/WebCrawler/WebCrawlerGroup/CrawlerInstances" />
            <sCSPolicyUpdateDomainMoniker name="/WebCrawler/WebCrawlerGroup/CrawlerUpgradeDomains" />
            <sCSPolicyFaultDomainMoniker name="/WebCrawler/WebCrawlerGroup/CrawlerFaultDomains" />
          </sCSPolicy>
        </groupHascomponents>
        <groupHascomponents>
          <role name="Dashboard" generation="1" functional="0" release="0" software="C:\Users\Kevin\Documents\Visual Studio 2012\Projects\WebCrawler\WebCrawler\csx\Debug\roles\Dashboard" entryPoint="base\x64\WaHostBootstrapper.exe" parameters="base\x64\WaIISHost.exe " memIndex="1792" hostingEnvironment="frontendadmin" hostingEnvironmentVersion="2">
            <componentports>
              <inPort name="Endpoint1" protocol="http" portRanges="80" />
            </componentports>
            <settings>
              <aCS name="Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="" />
              <aCS name="__ModelData" defaultValue="&lt;m role=&quot;Dashboard&quot; xmlns=&quot;urn:azure:m:v1&quot;&gt;&lt;r name=&quot;Crawler&quot; /&gt;&lt;r name=&quot;Dashboard&quot;&gt;&lt;e name=&quot;Endpoint1&quot; /&gt;&lt;/r&gt;&lt;/m&gt;" />
            </settings>
            <resourcereferences>
              <resourceReference name="DiagnosticStore" defaultAmount="[4096,4096,4096]" defaultSticky="true" kind="Directory" />
              <resourceReference name="EventStore" defaultAmount="[1000,1000,1000]" defaultSticky="false" kind="LogStore" />
            </resourcereferences>
          </role>
          <sCSPolicy>
            <sCSPolicyIDMoniker name="/WebCrawler/WebCrawlerGroup/DashboardInstances" />
            <sCSPolicyUpdateDomainMoniker name="/WebCrawler/WebCrawlerGroup/DashboardUpgradeDomains" />
            <sCSPolicyFaultDomainMoniker name="/WebCrawler/WebCrawlerGroup/DashboardFaultDomains" />
          </sCSPolicy>
        </groupHascomponents>
      </components>
      <sCSPolicy>
        <sCSPolicyUpdateDomain name="DashboardUpgradeDomains" defaultPolicy="[5,5,5]" />
        <sCSPolicyUpdateDomain name="CrawlerUpgradeDomains" defaultPolicy="[5,5,5]" />
        <sCSPolicyFaultDomain name="CrawlerFaultDomains" defaultPolicy="[2,2,2]" />
        <sCSPolicyFaultDomain name="DashboardFaultDomains" defaultPolicy="[2,2,2]" />
        <sCSPolicyID name="CrawlerInstances" defaultPolicy="[1,1,1]" />
        <sCSPolicyID name="DashboardInstances" defaultPolicy="[1,1,1]" />
      </sCSPolicy>
    </group>
  </groups>
  <implements>
    <implementation Id="282d6151-919a-4ed8-b500-b263ab1b0a1d" ref="Microsoft.RedDog.Contract\ServiceContract\WebCrawlerContract@ServiceDefinition">
      <interfacereferences>
        <interfaceReference Id="60c653bc-e462-44de-8209-0df9ead49bb5" ref="Microsoft.RedDog.Contract\Interface\Dashboard:Endpoint1@ServiceDefinition">
          <inPort>
            <inPortMoniker name="/WebCrawler/WebCrawlerGroup/Dashboard:Endpoint1" />
          </inPort>
        </interfaceReference>
      </interfacereferences>
    </implementation>
  </implements>
</serviceModel>