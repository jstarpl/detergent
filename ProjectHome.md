## Introduction ##
Detergent is a C# library providing a simple interface-based abstraction layer for implementing SOAP and REST Web services and it also contains a simplified MVC framework.

The abstraction allows running Web applications either through HTTP handlers (through IIS) or HTTP listeners (as a standalone Web server) using the same application code.

## Why? ##
The library was created when its author grew very wary of WCF's over-designed and complicated architecture.

## Why The Name Detergent? ##
It's like soap, but it's more effective ;)

## How To Use ##
Here's a sample unit test which runs a **very** simple REST web service (you can find the source code in the Detergent.Test project source code):
```
        [Test]
        public void Test()
        {
            using (WebClient webClient = new WebClient())
            {
                string response = webClient.DownloadString(
                    "http://localhost:9999/service/packages#fragment?page=2");
                Assert.IsFalse(string.IsNullOrEmpty(response));
                Assert.AreEqual("list", response);
            }
        }

        [SetUp]
        private void Setup()
        {
            // we will use REST web services for this sample
            DetergentRestHttpHandler httpHandler = new DetergentRestHttpHandler();

            // create a HTTP listener so we can run the test without IIS
            listener = new DetergentHttpListener(
                "http://localhost:9999",
                "/service",
                httpHandler);

            // set up REST URL routing for our HTTP REST handler
            SimpleRestRequestRouter router = new SimpleRestRequestRouter();
            httpHandler.RequestRouter = router;

            // create our sample REST service...
            SampleRestService service = new SampleRestService();
            // ... and configure routing to it
            service.ConfigureRoutes(router);
        }

        [TearDown]
        private void Teardown()
        {
            // close the HTTP listener
            if (listener != null)
            {
                listener.Dispose();
                listener = null;
            }
        }

        private DetergentHttpListener listener;
```

And here's the code for our simple REST service. It doesn't do much:
```
    public class SampleRestService
    {
        public void ConfigureRoutes(SimpleRestRequestRouter router)
        {
            router.AddRoute(HttpMethod.GET, "packages$", ListPackages);
            router.AddRoute(HttpMethod.GET, "package/(?<id>[^//]+)$", GetPackage);            
        }

        private static IHttpResponse GetPackage(IHttpContext context, RouteMatchData routeMatchData)
        {
            return LiteralHttpResponse.FormatHttpResponse(
                HttpStatusCode.OK,
                HttpConstants.ContentTypeText,
                "get{0}",
                routeMatchData.GetParameter("id"));
        }

        private static IHttpResponse ListPackages(IHttpContext context, RouteMatchData routeMatchData)
        {
            return LiteralHttpResponse.FormatHttpResponse(
                HttpStatusCode.OK, 
                HttpConstants.ContentTypeText,
                "list");
        }        
    }
```

## How To Build ##
  1. Download the source code.
  1. Open a command line in the **trunk** directory.
  1. Run **Build.bat** file.
  1. This will prepare all the files necessary for compiling the solution within the Visual Studio.
  1. It will also run unit tests...
  1. ... and package the binaries inside the **Builds\Packages** directory.