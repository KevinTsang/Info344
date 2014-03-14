<?xml version="1.0" encoding="utf-8"?>
<serviceModel xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" name="Info344Final" generation="1" functional="0" release="0" Id="5e1757f9-2771-43fe-872d-9b1b8693b211" dslVersion="1.2.0.0" xmlns="http://schemas.microsoft.com/dsltools/RDSM">
  <groups>
    <group name="Info344FinalGroup" generation="1" functional="0" release="0">
      <componentports>
        <inPort name="Dashboard:Endpoint1" protocol="http">
          <inToChannel>
            <lBChannelMoniker name="/Info344Final/Info344FinalGroup/LB:Dashboard:Endpoint1" />
          </inToChannel>
        </inPort>
      </componentports>
      <settings>
        <aCS name="Dashboard:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/Info344Final/Info344FinalGroup/MapDashboard:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </maps>
        </aCS>
        <aCS name="DashboardInstances" defaultValue="[1,1,1]">
          <maps>
            <mapMoniker name="/Info344Final/Info344FinalGroup/MapDashboardInstances" />
          </maps>
        </aCS>
        <aCS name="WebCrawler:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/Info344Final/Info344FinalGroup/MapWebCrawler:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </maps>
        </aCS>
        <aCS name="WebCrawlerInstances" defaultValue="[1,1,1]">
          <maps>
            <mapMoniker name="/Info344Final/Info344FinalGroup/MapWebCrawlerInstances" />
          </maps>
        </aCS>
      </settings>
      <channels>
        <lBChannel name="LB:Dashboard:Endpoint1">
          <toPorts>
            <inPortMoniker name="/Info344Final/Info344FinalGroup/Dashboard/Endpoint1" />
          </toPorts>
        </lBChannel>
      </channels>
      <maps>
        <map name="MapDashboard:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/Info344Final/Info344FinalGroup/Dashboard/Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </setting>
        </map>
        <map name="MapDashboardInstances" kind="Identity">
          <setting>
            <sCSPolicyIDMoniker name="/Info344Final/Info344FinalGroup/DashboardInstances" />
          </setting>
        </map>
        <map name="MapWebCrawler:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/Info344Final/Info344FinalGroup/WebCrawler/Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </setting>
        </map>
        <map name="MapWebCrawlerInstances" kind="Identity">
          <setting>
            <sCSPolicyIDMoniker name="/Info344Final/Info344FinalGroup/WebCrawlerInstances" />
          </setting>
        </map>
      </maps>
      <components>
        <groupHascomponents>
          <role name="Dashboard" generation="1" functional="0" release="0" software="C:\Users\Kevin\documents\visual studio 2012\Projects\Info344Final\Info344Final\csx\Debug\roles\Dashboard" entryPoint="base\x64\WaHostBootstrapper.exe" parameters="base\x64\WaIISHost.exe " memIndex="1792" hostingEnvironment="frontendadmin" hostingEnvironmentVersion="2">
            <componentports>
              <inPort name="Endpoint1" protocol="http" portRanges="80" />
            </componentports>
            <settings>
              <aCS name="Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="" />
              <aCS name="__ModelData" defaultValue="&lt;m role=&quot;Dashboard&quot; xmlns=&quot;urn:azure:m:v1&quot;&gt;&lt;r name=&quot;Dashboard&quot;&gt;&lt;e name=&quot;Endpoint1&quot; /&gt;&lt;/r&gt;&lt;r name=&quot;WebCrawler&quot; /&gt;&lt;/m&gt;" />
            </settings>
            <resourcereferences>
              <resourceReference name="DiagnosticStore" defaultAmount="[4096,4096,4096]" defaultSticky="true" kind="Directory" />
              <resourceReference name="EventStore" defaultAmount="[1000,1000,1000]" defaultSticky="false" kind="LogStore" />
            </resourcereferences>
          </role>
          <sCSPolicy>
            <sCSPolicyIDMoniker name="/Info344Final/Info344FinalGroup/DashboardInstances" />
            <sCSPolicyUpdateDomainMoniker name="/Info344Final/Info344FinalGroup/DashboardUpgradeDomains" />
            <sCSPolicyFaultDomainMoniker name="/Info344Final/Info344FinalGroup/DashboardFaultDomains" />
          </sCSPolicy>
        </groupHascomponents>
        <groupHascomponents>
          <role name="WebCrawler" generation="1" functional="0" release="0" software="C:\Users\Kevin\documents\visual studio 2012\Projects\Info344Final\Info344Final\csx\Debug\roles\WebCrawler" entryPoint="base\x64\WaHostBootstrapper.exe" parameters="base\x64\WaWorkerHost.exe " memIndex="1792" hostingEnvironment="consoleroleadmin" hostingEnvironmentVersion="2">
            <settings>
              <aCS name="Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="" />
              <aCS name="__ModelData" defaultValue="&lt;m role=&quot;WebCrawler&quot; xmlns=&quot;urn:azure:m:v1&quot;&gt;&lt;r name=&quot;Dashboard&quot;&gt;&lt;e name=&quot;Endpoint1&quot; /&gt;&lt;/r&gt;&lt;r name=&quot;WebCrawler&quot; /&gt;&lt;/m&gt;" />
            </settings>
            <resourcereferences>
              <resourceReference name="DiagnosticStore" defaultAmount="[4096,4096,4096]" defaultSticky="true" kind="Directory" />
              <resourceReference name="EventStore" defaultAmount="[1000,1000,1000]" defaultSticky="false" kind="LogStore" />
            </resourcereferences>
          </role>
          <sCSPolicy>
            <sCSPolicyIDMoniker name="/Info344Final/Info344FinalGroup/WebCrawlerInstances" />
            <sCSPolicyUpdateDomainMoniker name="/Info344Final/Info344FinalGroup/WebCrawlerUpgradeDomains" />
            <sCSPolicyFaultDomainMoniker name="/Info344Final/Info344FinalGroup/WebCrawlerFaultDomains" />
          </sCSPolicy>
        </groupHascomponents>
      </components>
      <sCSPolicy>
        <sCSPolicyUpdateDomain name="DashboardUpgradeDomains" defaultPolicy="[5,5,5]" />
        <sCSPolicyUpdateDomain name="WebCrawlerUpgradeDomains" defaultPolicy="[5,5,5]" />
        <sCSPolicyFaultDomain name="DashboardFaultDomains" defaultPolicy="[2,2,2]" />
        <sCSPolicyFaultDomain name="WebCrawlerFaultDomains" defaultPolicy="[2,2,2]" />
        <sCSPolicyID name="DashboardInstances" defaultPolicy="[1,1,1]" />
        <sCSPolicyID name="WebCrawlerInstances" defaultPolicy="[1,1,1]" />
      </sCSPolicy>
    </group>
  </groups>
  <implements>
    <implementation Id="8e411223-6268-40fa-ab9c-cf6b2fd18e96" ref="Microsoft.RedDog.Contract\ServiceContract\Info344FinalContract@ServiceDefinition">
      <interfacereferences>
        <interfaceReference Id="512dd6fe-00f1-4b7b-afc4-d5a670507d2c" ref="Microsoft.RedDog.Contract\Interface\Dashboard:Endpoint1@ServiceDefinition">
          <inPort>
            <inPortMoniker name="/Info344Final/Info344FinalGroup/Dashboard:Endpoint1" />
          </inPort>
        </interfaceReference>
      </interfacereferences>
    </implementation>
  </implements>
</serviceModel>