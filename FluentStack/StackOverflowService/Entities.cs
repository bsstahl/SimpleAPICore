using System;
using System.Collections.Generic;
using System.Linq;
using StackOverflow.Model;

namespace StackOverflowService
{
    public class Entities
    {
        private IEnumerable<Post> _posts;
        public IEnumerable<Post> Posts
        {
            get { return _posts; }
        }

        public Entities(Uri serviceRoot)
        {
            // Posts are loaded using the hard-coded values in the LoadPosts method below
            // They are then updated based on the current date as if they were recent
            // posts so that the demos using "Last 30 Days" work properly
            // If this had been an actual service proxy, the serviceRoot value would be used
            // to configure the location of the service endpoint
            _posts = LoadPosts();
            UpdatePostDates(_posts);
        }

        private void UpdatePostDates(IEnumerable<Post> posts)
        {
            var lastDate = posts.Max(p => p.CreationDate);
            var daysToAdd = DateTime.Now.Subtract(lastDate).TotalDays - 23;

            foreach (var post in posts)
            {
                post.CreationDate = post.CreationDate.AddDays(daysToAdd);
            }
        }

        private IEnumerable<Post> LoadPosts()
        {
            var posts = new PostCollection();

            posts.Add(
            id: 18593314,
            title: "Implementing ODataController with custom routing returns &quot;The related entity set could not be found from the OData path&quot; error",
            parentId: null,
            acceptedAnswerId: null,
            creationDate: DateTime.Parse("2013-09-11T07:22:10"),
            body: "I'm trying to move from our current implementation of ApiController to ODataController since it's the only way I found possible to enable returning OData Json formatted data. (Same with the problem here but the solution didn't work for me) I've been trying to workout ODataController and I found it working well enough following this. However, my project implements a different routing from the default OData routing of the simple \"~/odata/Entity\". I need to group my controllers into areas since there are some controllers that duplicates in name. Following this and this, I was able to implement custom routing and running it seems to reach the right controller and pass through it successfully.",
            tags: "<odata>");

            posts.Add(
            id: 18755469,
            title: "Implementing ODataController with custom routing returns &quot;The related entity set could not be found from the OData path&quot; error",
            parentId: 18593314,
            acceptedAnswerId: null,
            creationDate: DateTime.Parse("2013-09-12T04:38:12"),
            body: "<p>Eventually solved this just by removing the {controller} part of the routePrefix and implementing a custom <strong>IODataRoutingConvention</strong> to enter a default controller value when the initial parsing of the route doesn't detect a controller value. Code is as follows:</p>      <pre><code>public string SelectController(ODataPath odataPath, HttpRequestMessage request)      {      string controllerName = \"\"      //do stuff here to select controller      return controllerName;      }      </code></pre>      <p>I'm guessing this is the long substitute for the default and the contraints parameters in ApiController's map routing</p>      <pre><code>RouteTable.Routes.MapHttpRoute(      name: \"RouteName\",      routeTemplate: \"{version}/{area}/{controller}\",      defaults: new { controller = controllerName },      constraints: new { version = v1 }      );      </code></pre>      <p><strong>Note:</strong> Posting this as answer as of now but if anyone has a better explanation, I'll check it up. :)</p>",
            tags: "");

            posts.Add(
            id: 18569779,
            title: "Implementing $select with WebApi and ODataQueryOptions",
            parentId: null,
            acceptedAnswerId: 18603494,
            creationDate: DateTime.Parse("2013-09-03T19:19:15"),
            body: "<p>I'm trying to implement some OData functionality with a custom DAL using ODataQueryOptions but it isn't working.</p>",
            tags: "<odata><web-api>");

            posts.Add(
            id: 18603494,
            title: "Implementing $select with WebApi and ODataQueryOptions",
            parentId: 18569779,
            acceptedAnswerId: null,
            creationDate: DateTime.Parse("2013-09-04T00:19:35"),
            body: "<p>You need to do the same thing that <a href=\"https://aspnetwebstack.codeplex.com/SourceControl/latest#src/System.Web.Http.OData/OData/Query/SelectExpandQueryOption.cs\" rel=\"nofollow\">SelectExpandQueryOption.ApplyTo</a> does.</p>      <p>1) Optimize the query to the backend. Instead of getting the whole entity from the database, get only the properties the client asked for and wrap that result in an IEdmEntityObject. Return the collection as EdmEntityObjectCollection. This step is optional. You could choose to ignore this step and return IQueryable and still get $select to work.</p>      <p>2) Tell the OData formatter to serialize only the requested fields. This can be done by stuffing the SelectExpandClause on the Request object using the extension method <code>Request.SetSelectExpandClause</code>.</p>      <pre><code>public class CustomersController : ODataController      {      public IEnumerable&lt;Customer&gt; Get(ODataQueryOptions&lt;Customer&gt; query)      {      Customer[] customers = new[] { new Customer { ID = 42, Name = \"Raghu\" } };      // Apply query      var result = customers;      // set the SelectExpandClause on the request to hint the odata formatter to      // select/expand only the fields mentioned in the SelectExpandClause.      if (query.SelectExpand != null)      {      Request.SetSelectExpandClause(query.SelectExpand.SelectExpandClause);      }      return result;      }      }      </code></pre>",
            tags: "");

            posts.Add(
            id: 18637774,
            title: "Preventing modification of specific properties on entity",
            parentId: 18636687,
            acceptedAnswerId: null,
            creationDate: DateTime.Parse("2013-09-05T13:32:38"),
            body: "<p>You can do this with a ChangeInterceptor</p>      <pre><code>[ChangeInterceptor(\"Orders\")]      public void OnChangeOrders(Order order, UpdateOperations operations)      {      if (operations == UpdateOperations.Change)      {      //Get the record as it exists before the change is made      var oldValue CurrentDataSource.ChangeTracker.Entries&lt;Order&gt;().First();      //you can compare the various properties here to determine what if anything      //has changed or just write over the property if you want      order.CustomerID = oldValue.Entity.CustomerID;      }      }      </code></pre>",
            tags: "");

            posts.Add(
            id: 18639159,
            title: "ASP.NET Web Api: Correct way to serve OData-queryable GET requests",
            parentId: 18637906,
            acceptedAnswerId: null,
            creationDate: DateTime.Parse("2013-09-05T14:33:42"),
            body: "<p>You don't have to expose <code>IQueryable&lt;&gt;</code> - you can create a method that accepts an instance of <code>ODataQueryOptions</code> and process this yourself. Here's a code sample that does most of what you require. It should be more than enough for you to work out the solution that works best for you. This method will also allow you to keep your EF proxy classes.</p>      <pre><code>using System.Web.Http.OData;      using System.Web.Http.OData.Builder;      using System.Web.Http.OData.Query;      [ActionName(\"Dto\")]      public IList&lt;DtoModel&gt; GetDto(ODataQueryOptions&lt;DtoModel&gt; queryOptions)      {      var data2 = DatabaseData();      //Create a set of ODataQueryOptions for the internal class      ODataModelBuilder modelBuilder = new ODataConventionModelBuilder();      modelBuilder.EntitySet&lt;Model&gt;(\"Model\");      var queryContext = new ODataQueryContext(      modelBuilder.GetEdmModel(), typeof(Model));      var newQueryOptions = new ODataQueryOptions&lt;Model&gt;(queryContext, Request);      var t = new ODataValidationSettings() { MaxTop = 25 };      var s = new ODataQuerySettings() { PageSize = 25 };      newQueryOptions.Validate(t);      IEnumerable&lt;Model&gt; results =      (IEnumerable&lt;Model&gt;)newQueryOptions.ApplyTo(data2, s);      int skip = newQueryOptions.Skip == null ? 0 : newQueryOptions.Skip.Value;      int take = newQueryOptions.Top == null ? 25 : newQueryOptions.Top.Value;      IList&lt;Model&gt; internalResults = results.Skip(skip).Take(take).ToList();      // map from Model to Dto here using AutoMapper      AutoMapper.Mapper.CreateMap&lt;Model, DtoModel&gt;();      IList&lt;DtoModel&gt; webResults =      AutoMapper.Mapper.Map&lt;IList&lt;Model&gt;, IList&lt;DtoModel&gt;&gt;(internalResults);      return webResults;      }      </code></pre>      <p>The data used in the example is a simple <code>Queryable</code> set of data:</p>      <pre><code>private IQueryable&lt;Model&gt; DatabaseData()      {      return (      new Model[] {      new Model() { id = 1, name = \"one\", type = \"a\" },      new Model() { id = 2, name = \"two\", type = \"b\" },      new Model() { id = 3, name = \"three\", type = \"c\" },      new Model() { id = 4, name = \"four\", type = \"d\" },      new Model() { id = 5, name = \"five\", type = \"e\" },      new Model() { id = 6, name = \"six\", type = \"f\" },      new Model() { id = 7, name = \"seven\", type = \"g\" },      new Model() { id = 8, name = \"eight\", type = \"h\" },      new Model() { id = 9, name = \"nine\", type = \"i\" }      })      .AsQueryable();      }      </code></pre>      <p>These are the test classes:</p>      <pre><code>public class Poco      {      public int id { get; set; }      public string name { get; set; }      public string type { get; set; }      }      public class DtoModel      {      public int id { get; set; }      public string name { get; set; }      public string type { get; set; }      }      public class Model      {      public int id { get; set; }      public string name { get; set; }      public string type { get; set; }      public virtual ICollection&lt;Poco&gt; Pocos { get; set; }      }      </code></pre>",
            tags: "");

            posts.Add(
            id: 18483510,
            title: "WCF ado.net Insert record (at end)",
            parentId: null,
            acceptedAnswerId: null,
            creationDate: DateTime.Parse("2013-09-03T11:08:25"),
            body: "<p>Stackers, I have been troubled with this problem for so long that I think it finally broke my will to live. I am wring an app for windowsphone 8. The app revolves around a thing that I don't get.</p>",
            tags: ""
            );

            posts.Add(
            id: 18590768,
            title: "WCF ado.net Insert record (at end)",
            parentId: 18483510,
            acceptedAnswerId: null,
            creationDate: DateTime.Parse("2013-09-03T11:09:58"),
            body: "<p>Okay, this was really stupid. In my mssql table, i had not enabled the feature to add rows with auto increment. Atleast, i had it enabled but the increment was set to 0. So it would point to the same record. </p>",
            tags: ""
            );

            posts.Add(
            id: 18665715,
            title: "AngularJS - handling RESTful $resource response",
            parentId: null,
            acceptedAnswerId: 18665982,
            creationDate: DateTime.Parse("2013-09-06T20:00:20"),
            body: "<p>I have an restful api and angularjs app.      I am using $resource inside a factory to work with this api.      I have a problem with one request. I POST my api to create some elements.</p>      <pre><code>/api/service/thing/make-things      </code></pre>      <p>I need to pass in my request some data. Here is what I am doing:</p>      <pre><code>$scope.someRequest = new SomeRequest(); // factory object returning an $resource      $scope.someRequest.some_field = 'abc';      $scope.someRequest.$save({someAdditionalParams:'123'}, function(values){...handling response...});      </code></pre>      <p>It works fine and POSTs data I want to post, but in this particular case my post response is array of objects.</p>      <pre><code>[{somestuff:'123'}, {somestuff:'321'} ... ]      </code></pre>      <p>Angular tries to map it back to an object and throws me an error that object was expected but got an array. I tried to create a separate resource method with isArray:1, but it still failed with same kind of error.</p>      <p>So, my question is: how to handle this situation? Is it possible to cancel copying $save result to $resource object?</p>",
            tags: "<javascript><rest><angularjs>"
            );

            posts.Add(
            id: 18665314,
            title: "How to build url hierarchy for project composed with decoupled Backbone.js apps?",
            parentId: null,
            acceptedAnswerId: null,
            creationDate: DateTime.Parse("2013-09-06T19:34:52"),
            body: "<p>I like the way how Django do subj. It allows to define 'local' url inside apps, and then include them in project-level root url config and define prefixes for them. <br>      Like - local urls are: <code>that/&lt;id&gt;</code>, <code>this/&lt;slug&gt;/</code> and global prefix for entire app - <code>verygoodapp/</code>. </p>      <p>In Backbone.js this can be done through <code>root</code> parameter for <code>History.start()</code> function. </p>      <p>But in this case I will need to do <br>      <code>History.start({root: arbitraryUrlPrefixForThatAppPassedAsArgFromAboveLocalSpace})</code>      <br>      in every applciation. <br>      Is it ok? How js gurus do that?</p>      <p>upd<br>      I wrote that and realized that probably I need just write:</p>      <pre><code>function(urlRoot) {      var MyRouter = Backbone.Router.extend({      routes: {      urlRoot + 'content/:slug': 'openArticle'      },      ...      });      }      </code></pre>      <p>And do <code>History.start()</code> just once in global project space. Not tested yet.      <br>upd2<br>      Now I think that <a href=\"https://docs.djangoproject.com/en/dev/topics/http/urls/#reverse-resolution-of-urls\" rel=\"nofollow\">reverse resolution of urls</a> like in Django would also be cool to implement with Backbone. AFAIK Backbone Routers do register their routes in some centralized  internal facility.</p>",
            tags: "<javascript><rest><web-applications><backbone.js><backbone-routing>"
            );

            posts.Add(
            id: 18665243,
            title: "Create Subscription in a Topic programatically using Windows Azure Service Bus REST API",
            parentId: null,
            acceptedAnswerId: null,
            creationDate: DateTime.Parse("2013-09-06T19:30:57"),
            body: "<p>Is it possible to create a Subscription in an existing Topic programmatically in Azure Service Bus using the REST API?</p>      <p>I would like to create one on the fly and then delete programmatically as well.</p>",
            tags: "<rest><azure><azureservicebus>"
            );

            posts.Add(
            id: 18660487,
            title: "Consume application/json in REST services using Jersey in Osgi",
            parentId: null,
            acceptedAnswerId: null,
            creationDate: DateTime.Parse("2013-09-06T14:45:05"),
            body: "<p>I'm deploying some REST services in an Osgi bundle using Jersey. The services are notated like </p>      <pre><code>@POST      @Path(\"/adduser\")      @Consumes(MediaType.APPLICATION_XML+\",\"+MediaType.APPLICATION_JSON)      @Produces(MediaType.APPLICATION_XML)      public Message addUser(User user) {      ...      }      </code></pre>      <p>The first problem I have is that the service doesn't accept both of the MIME types I put into the @Consumes notation but just the first one.</p>      <p>The second and worst is that receive the following exeception when I try to call to the services. I can @Consumes text/plain and I can @Produces application/xml for example but if I try to @Consumes an application/json or application/xml the server throw the exception.</p>      <p>The exception is throwed when I make the call with a wellformed json or xml using a REST client or an ajax call, if the service just receive text/plain or doesnt receive anything the response to the client is made correctly in xml so the serializer is working ok when I send POJO's but not receiving them.</p>      <pre><code> javax.servlet.ServletException: org.glassfish.jersey.server.ContainerException: java.lang.LinkageError: loader constraint violation: when resolving interface method \"javax.xml.bind.Unmarshaller.unmarshal(Ljavax/xml/transform/Source;)Ljava/lang/Object;\" the class loader (instance of org/eclipse/osgi/internal/baseadaptor/DefaultClassLoader) of the current class, org/glassfish/jersey/message/internal/XmlRootElementJaxbProvider, and the class loader (instance of &lt;bootloader&gt;) for resolved class, javax/xml/bind/Unmarshaller, have different Class objects for the type ject; used in the signature      at org.glassfish.jersey.servlet.WebComponent.service(WebComponent.java:353)      at org.glassfish.jersey.servlet.ServletContainer.service(ServletContainer.java:372)      at org.glassfish.jersey.servlet.ServletContainer.service(ServletContainer.java:335)      at org.glassfish.jersey.servlet.ServletContainer.service(ServletContainer.java:218)      at org.apache.felix.http.base.internal.handler.ServletHandler.doHandle(ServletHandler.java:96)      at org.apache.felix.http.base.internal.handler.ServletHandler.handle(ServletHandler.java:79)      at org.apache.felix.http.base.internal.dispatch.ServletPipeline.handle(ServletPipeline.java:42)      at org.apache.felix.http.base.internal.dispatch.InvocationFilterChain.doFilter(InvocationFilterChain.java:49)      at org.apache.felix.http.base.internal.dispatch.HttpFilterChain.doFilter(HttpFilterChain.java:33)      at es.upm.cedint.gateway.api.corssupport.CORSFilter.doFilter(CORSFilter.java:164)      at es.upm.cedint.gateway.api.corssupport.CORSFilter.doFilter(CORSFilter.java:246)      at org.apache.felix.http.base.internal.handler.FilterHandler.doHandle(FilterHandler.java:88)      at org.apache.felix.http.base.internal.handler.FilterHandler.handle(FilterHandler.java:76)      at org.apache.felix.http.base.internal.dispatch.InvocationFilterChain.doFilter(InvocationFilterChain.java:47)      at org.apache.felix.http.base.internal.dispatch.HttpFilterChain.doFilter(HttpFilterChain.java:33)      at es.upm.cedint.gateway.api.security.SecurityFilter.doFilter(SecurityFilter.java:87)      at org.apache.felix.http.base.internal.handler.FilterHandler.doHandle(FilterHandler.java:88)      at org.apache.felix.http.base.internal.handler.FilterHandler.handle(FilterHandler.java:76)      at org.apache.felix.http.base.internal.dispatch.InvocationFilterChain.doFilter(InvocationFilterChain.java:47)      at org.apache.felix.http.base.internal.dispatch.HttpFilterChain.doFilter(HttpFilterChain.java:33)      at org.apache.felix.http.base.internal.dispatch.FilterPipeline.dispatch(FilterPipeline.java:48)      at org.apache.felix.http.base.internal.dispatch.Dispatcher.dispatch(Dispatcher.java:39)      at org.apache.felix.http.base.internal.DispatcherServlet.service(DispatcherServlet.java:67)      at javax.servlet.http.HttpServlet.service(HttpServlet.java:848)      at org.mortbay.jetty.servlet.ServletHolder.handle(ServletHolder.java:511)      at org.mortbay.jetty.servlet.ServletHandler.handle(ServletHandler.java:390)      at org.mortbay.jetty.servlet.SessionHandler.handle(SessionHandler.java:182)      at org.mortbay.jetty.handler.ContextHandler.handle(ContextHandler.java:765)      at org.mortbay.jetty.handler.HandlerWrapper.handle(HandlerWrapper.java:152)      at org.mortbay.jetty.Server.handle(Server.java:326)      at org.mortbay.jetty.HttpConnection.handleRequest(HttpConnection.java:542)      at org.mortbay.jetty.HttpConnection$RequestHandler.content(HttpConnection.java:943)      at org.mortbay.jetty.HttpParser.parseNext(HttpParser.java:756)      at org.mortbay.jetty.HttpParser.parseAvailable(HttpParser.java:218)      at org.mortbay.jetty.HttpConnection.handle(HttpConnection.java:404)      at org.mortbay.io.nio.SelectChannelEndPoint.run(SelectChannelEndPoint.java:410)      at org.mortbay.thread.QueuedThreadPool$PoolThread.run(QueuedThreadPool.java:582)      Caused by: org.glassfish.jersey.server.ContainerException: java.lang.LinkageError: loader constraint violation: when resolving interface method \"javax.xml.bind.Unmarshaller.unmarshal(Ljavax/xml/transform/Source;)Ljava/lang/Object;\" the class loader (instance of org/eclipse/osgi/internal/baseadaptor/DefaultClassLoader) of the current class, org/glassfish/jersey/message/internal/XmlRootElementJaxbProvider, and the class loader (instance of &lt;bootloader&gt;) for resolved class, javax/xml/bind/Unmarshaller, have different Class objects for the type ject; used in the signature      at org.glassfish.jersey.servlet.internal.ResponseWriter.rethrow(ResponseWriter.java:230)      at org.glassfish.jersey.servlet.internal.ResponseWriter.failure(ResponseWriter.java:212)      at org.glassfish.jersey.server.ServerRuntime$Responder.process(ServerRuntime.java:401)      at org.glassfish.jersey.server.ServerRuntime$1.run(ServerRuntime.java:243)      at org.glassfish.jersey.internal.Errors$1.call(Errors.java:271)      at org.glassfish.jersey.internal.Errors$1.call(Errors.java:267)      at org.glassfish.jersey.internal.Errors.process(Errors.java:315)      at org.glassfish.jersey.internal.Errors.process(Errors.java:297)      at org.glassfish.jersey.internal.Errors.process(Errors.java:267)      at org.glassfish.jersey.process.internal.RequestScope.runInScope(RequestScope.java:322)      at org.glassfish.jersey.server.ServerRuntime.process(ServerRuntime.java:211)      at org.glassfish.jersey.server.ApplicationHandler.handle(ApplicationHandler.java:979)      at org.glassfish.jersey.servlet.WebComponent.service(WebComponent.java:344)      ... 36 more      Caused by: java.lang.LinkageError: loader constraint violation: when resolving interface method \"javax.xml.bind.Unmarshaller.unmarshal(Ljavax/xml/transform/Source;)Ljava/lang/Object;\" the class loader (instance of org/eclipse/osgi/internal/baseadaptor/DefaultClassLoader) of the current class, org/glassfish/jersey/message/internal/XmlRootElementJaxbProvider, and the class loader (instance of &lt;bootloader&gt;) for resolved class, javax/xml/bind/Unmarshaller, have different Class objects for the type ject; used in the signature      at org.glassfish.jersey.message.internal.XmlRootElementJaxbProvider.readFrom(XmlRootElementJaxbProvider.java:140)      at org.glassfish.jersey.message.internal.AbstractRootElementJaxbProvider.readFrom(AbstractRootElementJaxbProvider.java:122)      at org.glassfish.jersey.message.internal.ReaderInterceptorExecutor$TerminalReaderInterceptor.aroundReadFrom(ReaderInterceptorExecutor.java:181)      at org.glassfish.jersey.message.internal.ReaderInterceptorExecutor.proceed(ReaderInterceptorExecutor.java:134)      at org.glassfish.jersey.server.internal.MappableExceptionWrapperInterceptor.aroundReadFrom(MappableExceptionWrapperInterceptor.java:72)      at org.glassfish.jersey.message.internal.ReaderInterceptorExecutor.proceed(ReaderInterceptorExecutor.java:134)      at org.glassfish.jersey.message.internal.MessageBodyFactory.readFrom(MessageBodyFactory.java:988)      at org.glassfish.jersey.message.internal.InboundMessageContext.readEntity(InboundMessageContext.java:833)      at org.glassfish.jersey.server.ContainerRequest.readEntity(ContainerRequest.java:252)      at org.glassfish.jersey.server.internal.inject.EntityParamValueFactoryProvider$EntityValueFactory.get(EntityParamValueFactoryProvider.java:96)      at org.glassfish.jersey.server.internal.inject.AbstractHttpContextValueFactory.provide(AbstractHttpContextValueFactory.java:66)      at org.glassfish.jersey.server.spi.internal.ParameterValueHelper.getParameterValues(ParameterValueHelper.java:81)      at org.glassfish.jersey.server.model.internal.JavaResourceMethodDispatcherProvider$AbstractMethodParamInvoker.getParamValues(JavaResourceMethodDispatcherProvider.java:121)      at org.glassfish.jersey.server.model.internal.JavaResourceMethodDispatcherProvider$TypeOutInvoker.doDispatch(JavaResourceMethodDispatcherProvider.java:195)      at org.glassfish.jersey.server.model.internal.AbstractJavaResourceMethodDispatcher.dispatch(AbstractJavaResourceMethodDispatcher.java:94)      at org.glassfish.jersey.server.model.ResourceMethodInvoker.invoke(ResourceMethodInvoker.java:353)      at org.glassfish.jersey.server.model.ResourceMethodInvoker.apply(ResourceMethodInvoker.java:343)      at org.glassfish.jersey.server.model.ResourceMethodInvoker.apply(ResourceMethodInvoker.java:102)      at org.glassfish.jersey.server.ServerRuntime$1.run(ServerRuntime.java:237)      ... 45 more      </code></pre>      <p>Anyone have any idea of what could be wrong?</p>      <p>PD:I have @XMLRootElement at the begining of the User class that I want to serialize and the server I use is Jetty.</p>",
            tags: "<json><rest><jaxb><jersey><osgi>"
            );

            posts.Add(
            id: 18659048,
            title: "Hibernate LazyInitializationException thrown when called through Restful web service",
            parentId: null,
            acceptedAnswerId: null,
            creationDate: DateTime.Parse("2013-09-06T13:29:15"),
            body: "<p>I'm having a weird problem. I have implemented a feature in the project i'm working on using JSPs. Now  I have a new requirement where I have to implement the same feature as a REST service. I'm using the same Service and DAO methods that works perfectly fine with the JSP client but when i'm accessing it through the web service I'm getting LazyInitializationException on the mappings that I have. </p>      <p>Here is the Web Service GET method.</p>      <pre><code>@GET      @Path(\"/{test_id}\")      @Produces(\"application/json\")      public TestInfo getTestInfo(@PathParam(\"test_id\") long test_id) {      TestInfo info = null;      try {      info = service.getTest(test_id);      } catch (Exception e) {      e.printStackTrace();      }      return info;      }      </code></pre>      <p>The getTest() method works absolutely fine when the client is JSP. The stackTrace is as follows.</p>      <pre><code>org.hibernate.LazyInitializationException: failed to lazily initialize a collection of role: com.spaneos.ias.pojo.Question.answers, no session or session was closed      at org.hibernate.collection.AbstractPersistentCollection.throwLazyInitializationException(AbstractPersistentCollection.java:383)      at org.hibernate.collection.AbstractPersistentCollection.throwLazyInitializationExceptionIfNotConnected(AbstractPersistentCollection.java:375)      at org.hibernate.collection.AbstractPersistentCollection.initialize(AbstractPersistentCollection.java:368)      at org.hibernate.collection.AbstractPersistentCollection.read(AbstractPersistentCollection.java:111)      at org.hibernate.collection.PersistentSet.iterator(PersistentSet.java:186)      at com.sun.xml.internal.bind.v2.runtime.reflect.Lister$CollectionLister.iterator(Lister.java:267)      at com.sun.xml.internal.bind.v2.runtime.reflect.Lister$CollectionLister.iterator(Lister.java:254)      at com.sun.xml.internal.bind.v2.runtime.property.ArrayElementProperty.serializeListBody(ArrayElementProperty.java:118)      at com.sun.xml.internal.bind.v2.runtime.property.ArrayERProperty.serializeBody(ArrayERProperty.java:144)      at com.sun.xml.internal.bind.v2.runtime.ClassBeanInfoImpl.serializeBody(ClassBeanInfoImpl.java:343)      at com.sun.xml.internal.bind.v2.runtime.XMLSerializer.childAsXsiType(XMLSerializer.java:685)      at com.sun.xml.internal.bind.v2.runtime.property.SingleElementNodeProperty.serializeBody(SingleElementNodeProperty.java:143)      at com.sun.xml.internal.bind.v2.runtime.ClassBeanInfoImpl.serializeBody(ClassBeanInfoImpl.java:343)      at com.sun.xml.internal.bind.v2.runtime.XMLSerializer.childAsXsiType(XMLSerializer.java:685)      at com.sun.xml.internal.bind.v2.runtime.property.ArrayElementNodeProperty.serializeItem(ArrayElementNodeProperty.java:54)      at com.sun.xml.internal.bind.v2.runtime.property.ArrayElementProperty.serializeListBody(ArrayElementProperty.java:157)      at com.sun.xml.internal.bind.v2.runtime.property.ArrayERProperty.serializeBody(ArrayERProperty.java:144)      at com.sun.xml.internal.bind.v2.runtime.ClassBeanInfoImpl.serializeBody(ClassBeanInfoImpl.java:343)      at com.sun.xml.internal.bind.v2.runtime.XMLSerializer.childAsXsiType(XMLSerializer.java:685)      at com.sun.xml.internal.bind.v2.runtime.property.SingleElementNodeProperty.serializeBody(SingleElementNodeProperty.java:143)      at com.sun.xml.internal.bind.v2.runtime.ClassBeanInfoImpl.serializeBody(ClassBeanInfoImpl.java:343)      at com.sun.xml.internal.bind.v2.runtime.XMLSerializer.childAsSoleContent(XMLSerializer.java:582)      at com.sun.xml.internal.bind.v2.runtime.ClassBeanInfoImpl.serializeRoot(ClassBeanInfoImpl.java:325)      at com.sun.xml.internal.bind.v2.runtime.XMLSerializer.childAsRoot(XMLSerializer.java:483)      at com.sun.xml.internal.bind.v2.runtime.MarshallerImpl.write(MarshallerImpl.java:308)      at com.sun.xml.internal.bind.v2.runtime.MarshallerImpl.marshal(MarshallerImpl.java:163)      at com.sun.jersey.json.impl.BaseJSONMarshaller.marshallToJSON(BaseJSONMarshaller.java:106)      at com.sun.jersey.json.impl.provider.entity.JSONRootElementProvider.writeTo(JSONRootElementProvider.java:143)      at com.sun.jersey.core.provider.jaxb.AbstractRootElementProvider.writeTo(AbstractRootElementProvider.java:157)      at com.sun.jersey.spi.container.ContainerResponse.write(ContainerResponse.java:306)      at com.sun.jersey.server.impl.application.WebApplicationImpl._handleRequest(WebApplicationImpl.java:1479)      at com.sun.jersey.server.impl.application.WebApplicationImpl.handleRequest(WebApplicationImpl.java:1391)      at com.sun.jersey.server.impl.application.WebApplicationImpl.handleRequest(WebApplicationImpl.java:1381)      at com.sun.jersey.spi.container.servlet.WebComponent.service(WebComponent.java:416)      at com.sun.jersey.spi.container.servlet.ServletContainer.service(ServletContainer.java:538)      at com.sun.jersey.spi.container.servlet.ServletContainer.service(ServletContainer.java:716)      at javax.servlet.http.HttpServlet.service(HttpServlet.java:728)      at org.apache.catalina.core.ApplicationFilterChain.internalDoFilter(ApplicationFilterChain.java:305)      at org.apache.catalina.core.ApplicationFilterChain.doFilter(ApplicationFilterChain.java:210)      at org.springframework.security.web.FilterChainProxy$VirtualFilterChain.doFilter(FilterChainProxy.java:368)      at org.springframework.security.web.access.intercept.FilterSecurityInterceptor.invoke(FilterSecurityInterceptor.java:109)      at org.springframework.security.web.access.intercept.FilterSecurityInterceptor.doFilter(FilterSecurityInterceptor.java:83)      at org.springframework.security.web.FilterChainProxy$VirtualFilterChain.doFilter(FilterChainProxy.java:380)      at org.springframework.security.web.access.ExceptionTranslationFilter.doFilter(ExceptionTranslationFilter.java:97)      at org.springframework.security.web.FilterChainProxy$VirtualFilterChain.doFilter(FilterChainProxy.java:380)      at org.springframework.security.web.session.SessionManagementFilter.doFilter(SessionManagementFilter.java:100)      at org.springframework.security.web.FilterChainProxy$VirtualFilterChain.doFilter(FilterChainProxy.java:380)      at org.springframework.security.web.authentication.AnonymousAuthenticationFilter.doFilter(AnonymousAuthenticationFilter.java:78)      at org.springframework.security.web.FilterChainProxy$VirtualFilterChain.doFilter(FilterChainProxy.java:380)      at org.springframework.security.web.servletapi.SecurityContextHolderAwareRequestFilter.doFilter(SecurityContextHolderAwareRequestFilter.java:54)      at org.springframework.security.web.FilterChainProxy$VirtualFilterChain.doFilter(FilterChainProxy.java:380)      at org.springframework.security.web.savedrequest.RequestCacheAwareFilter.doFilter(RequestCacheAwareFilter.java:35)      at org.springframework.security.web.FilterChainProxy$VirtualFilterChain.doFilter(FilterChainProxy.java:380)      at org.springframework.security.web.authentication.AbstractAuthenticationProcessingFilter.doFilter(AbstractAuthenticationProcessingFilter.java:187)      at org.springframework.security.web.FilterChainProxy$VirtualFilterChain.doFilter(FilterChainProxy.java:380)      at org.springframework.security.web.authentication.logout.LogoutFilter.doFilter(LogoutFilter.java:105)      at org.springframework.security.web.FilterChainProxy$VirtualFilterChain.doFilter(FilterChainProxy.java:380)      at org.springframework.security.web.context.SecurityContextPersistenceFilter.doFilter(SecurityContextPersistenceFilter.java:79)      at org.springframework.security.web.FilterChainProxy$VirtualFilterChain.doFilter(FilterChainProxy.java:380)      at org.springframework.security.web.session.ConcurrentSessionFilter.doFilter(ConcurrentSessionFilter.java:109)      at org.springframework.security.web.FilterChainProxy$VirtualFilterChain.doFilter(FilterChainProxy.java:380)      at org.springframework.security.web.FilterChainProxy.doFilter(FilterChainProxy.java:169)      at org.springframework.web.filter.DelegatingFilterProxy.invokeDelegate(DelegatingFilterProxy.java:346)      at org.springframework.web.filter.DelegatingFilterProxy.doFilter(DelegatingFilterProxy.java:259)      at org.apache.catalina.core.ApplicationFilterChain.internalDoFilter(ApplicationFilterChain.java:243)      at org.apache.catalina.core.ApplicationFilterChain.doFilter(ApplicationFilterChain.java:210)      at org.springframework.orm.hibernate3.support.OpenSessionInViewFilter.doFilterInternal(OpenSessionInViewFilter.java:233)      at org.springframework.web.filter.OncePerRequestFilter.doFilter(OncePerRequestFilter.java:107)      at org.apache.catalina.core.ApplicationFilterChain.internalDoFilter(ApplicationFilterChain.java:243)      at org.apache.catalina.core.ApplicationFilterChain.doFilter(ApplicationFilterChain.java:210)      at org.apache.catalina.core.StandardWrapperValve.invoke(StandardWrapperValve.java:222)      at org.apache.catalina.core.StandardContextValve.invoke(StandardContextValve.java:123)      at org.apache.catalina.authenticator.AuthenticatorBase.invoke(AuthenticatorBase.java:472)      at org.apache.catalina.core.StandardHostValve.invoke(StandardHostValve.java:171)      at org.apache.catalina.valves.ErrorReportValve.invoke(ErrorReportValve.java:99)      at org.apache.catalina.valves.AccessLogValve.invoke(AccessLogValve.java:936)      at org.apache.catalina.core.StandardEngineValve.invoke(StandardEngineValve.java:118)      at org.apache.catalina.connector.CoyoteAdapter.service(CoyoteAdapter.java:407)      at org.apache.coyote.http11.AbstractHttp11Processor.process(AbstractHttp11Processor.java:1004)      at org.apache.coyote.AbstractProtocol$AbstractConnectionHandler.process(AbstractProtocol.java:589)      at org.apache.tomcat.util.net.JIoEndpoint$SocketProcessor.run(JIoEndpoint.java:310)      at java.util.concurrent.ThreadPoolExecutor.runWorker(ThreadPoolExecutor.java:1145)      at java.util.concurrent.ThreadPoolExecutor$Worker.run(ThreadPoolExecutor.java:615)      at java.lang.Thread.run(Thread.java:724)      </code></pre>      <p>Can anyone please help how to return a mapped object (Loaded with many collections) using web service?      Please let me know if I have to give more details.      Thanks in advance. </p>",
            tags: "<hibernate><rest><spring-mvc><lazy-initialization>"
            );

            posts.Add(
            id: 18654880,
            title: "MOM characteristics in REST",
            parentId: null,
            acceptedAnswerId: null,
            creationDate: DateTime.Parse("2013-09-06T09:49:52"),
            body: "<p>Recently i read this interesting article (<a href=\"http://windyroad.com.au/2012/01/13/the-chain-of-custody-problem/\" rel=\"nofollow\">http://windyroad.com.au/2012/01/13/the-chain-of-custody-problem/</a> ) about comparison between REST and MOM.I know REST is architectural style. MOM's(JMS server e.g.) basic features include message persistance and patterns like PUB-SUB.      Can REST be used instead of MOM? I know it will vary from case to case but if 'yes', how one can gurantee message/data is not lost?</p>",
            tags: "<rest>"
            );

            posts.Add(
            id: 18650957,
            title: "Sequencing 2 REST API in wso2 esb with a decision",
            parentId: null,
            acceptedAnswerId: null,
            creationDate: DateTime.Parse("2013-09-06T06:03:15"),
            body: "<p>We have 3 REST Services deployed on a backend server.</p>      <pre><code>List&lt;Employee&gt; searchEmployees(EmployeeSearchParams params)      DecisionResult isEligibleForIncrement(Employee emp)      Employee incrementSalary(Employee emp)      </code></pre>      <p>The requirement is to sequence these services in ESB with the following logic.</p>      <p>SearchEmployees using searchEmployees.  For each employee, check for eligibility using isEligibleForIncrement, if eligible [this is determined using DecisionResult.resultCode], increment salary using incrementSalary.</p>      <p>We are new to WSO2 ESB, please help in sequencing this.</p>      <p>PS: We are able to invoke individual service using REST API, however we're finding difficult to orchestrate.  Please please help!</p>",
            tags: "<rest><sequence><wso2esb><orchestration><design-decisions>"
            );

            posts.Add(
            id: 18641083,
            title: "Is it RESTful to send user role in http response?",
            parentId: null,
            acceptedAnswerId: null,
            creationDate: DateTime.Parse("2013-09-05T16:05:36"),
            body: "<p>I'm trying to find a way to modify a web page based on the a user's authorization. For example, the owner of a resource should see <code>edit</code> and <code>delete</code> buttons while a regular user should not see those.</p>      <p>The following is a method I am thinking about, but am unsure if there is a more common/better way.</p>      <p>For example, assume there is a resource </p>      <pre><code>project: {      name: string,      owner_id: id,      moderators: [ids],      other_stuff: string      }      </code></pre>      <p>Would it be RESTful and good practice to extend this object with an attribute that describes what role the current logged in user is? For example, assuming this user is one of the moderators, it'd send</p>      <pre><code>project: {      name: string,      owner_id: id,      moderators: [ids],      other_stuff: string,      user_role: \"moderator\"      }      </code></pre>      <p>Then the client-side framework would use <code>project.user_role</code> to display role-based controls such as <code>delete</code> and <code>edit</code>.</p>      <p>Also, are there any good resources for me to learn more about this topic? I've scanned through some REST api design tutorials and books, but I haven't seen something like I've described above.</p>      <p>Thanks! </p>",
            tags: "<api><rest><authorization>"
            );

            posts.Add(
            id: 18640833,
            title: "Batch update using PUT - wcf data services / odata",
            parentId: null,
            acceptedAnswerId: null,
            creationDate: DateTime.Parse("2013-09-05T15:52:20"),
            body: "<p>I have a java odata service which uses PUT to update data. Client is in dotnet.</p>      <p>I have an object (order) which has an attribute \"list object\" (order details). </p>      <p>Is it possible to send Order object in one short with order details list using below command</p>      <pre><code>Order orderObj = new Order      OrderDetail oDetailObj = new OrderDetail      context.AttachTo(Orders, orderObj);      Code to set properties of orderObj      for loop to add orders details      {      Code to set properties of oDetailObj      context.AddRelatedObject(orderObj, \"OrderDetailsList\", oDetailObj);      }      DataServiceResponse response = context.SaveChanges(SaveChangesOptions.ReplaceOnUpdate);      </code></pre>      <p>While running this code, only last order detail infomration is submitting to server. I have verified that using fidller</p>      <p>if you have any alternative please suggest.</p>",
            tags: "<c#><java><odata><wcf-data-services-client>"
            );

            posts.Add(
            id: 18639538,
            title: "HTTP Status 500 - org.springframework.web.client.HttpClientErrorException: 404 /",
            parentId: null,
            acceptedAnswerId: null,
            creationDate: DateTime.Parse("2013-09-05T14:52:18"),
            body: "<p>I am using RestTemplate, but when i call postFor function i get following exception, Below are the code for showing detail:</p>      <p><strong>Controller</strong></p>      <pre><code>@Controller      @RequestMapping(\"/data\")      public class DataController {      @RequestMapping(value = \"/{id}\", method = RequestMethod.POST)      public ResponseEntity&lt;ManagementResource&gt; postData(@PathVariable(\"id\") String id,@RequestBody Data1 data) {      RSResponse&lt;Data1&gt; response = new RSResponse&lt;Data1&gt;();      response.setStatus(RSResponse.Status.SUCCESS);      response.setData(data);      return new ResponseEntity&lt;ManagementResource&gt;(HttpStatus.CREATED);      }      }      </code></pre>      <p><strong>client code:</strong></p>      <pre><code>RestTemplate rt = new RestTemplate();      Data1 d = new Data1();      d.setEmp_id(1);      d.setEmp_name(\"abc\");      d.setEmp_salary(10000);      Map&lt;String, String&gt; vars = new HashMap&lt;String, String&gt;();      vars.put(\"id\", \"JS01\");      String url = \"http://localhost:8090/JSFFaceletsTutorial/data/{id}\";      ResponseEntity&lt;Data1&gt; response = rt.postForEntity(url,d,Data1.class,vars);      Data1 data = response.getBody();      </code></pre>      <p>please tell if anyone knows it.      Thanks</p>",
            tags: "<rest><resttemplate>"
            );

            posts.Add(
            id: 18637402,
            title: "Is there a generic tool to import files into a Rest API",
            parentId: null,
            acceptedAnswerId: null,
            creationDate: DateTime.Parse("2013-09-05T13:16:18"),
            body: "<p>I need to create a way for users to bulk import data into our system.  We already have a nice Rest API to call for each item.  I am need to accept a file (CSV, XML, or whatever makes this easy), split it line by line (if CSV), and call the API for each line.  It seems like this is a fairly generic problem and there should be a tool that handles everything but the last step, but I haven't been able to find that tool.</p>",
            tags: "<rest><csv><import>"
            );

            posts.Add(
            id: 18634411,
            title: "How to make changes on the CXF configuration xml file on runtime",
            parentId: null,
            acceptedAnswerId: null,
            creationDate: DateTime.Parse("2013-09-05T10:52:38"),
            body: "<p>I have developed a web app that creates Rest Web Services dynamically on runtime. I am using Apache CXF and tomcat 6. My <code>web.xml</code> file looks like below</p>      <pre><code>&lt;?xml version=\"1.0\" encoding=\"utf-8\"?&gt;      &lt;web-app&gt;      &lt;context-param&gt;      &lt;param-name&gt;contextConfigLocation&lt;/param-name&gt;      &lt;param-value&gt;WEB-INF/beans.xml&lt;/param-value&gt;      &lt;/context-param&gt;      &lt;listener&gt;      &lt;listener-class&gt;org.springframework.web.context.ContextLoaderListener&lt;/listener-class&gt;      &lt;/listener&gt;      ...      ...      &lt;/web-app&gt;      </code></pre>      <p>and my beans.xml file looks like below</p>      <pre><code>&lt;?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"no\"?&gt;      &lt;beans&gt;      &lt;import resource=\"classpath:META-INF/cxf/cxf.xml\"/&gt;      &lt;jaxrs:server address=\"/\" id=\"services\"&gt;      &lt;jaxrs:serviceBeans&gt;      &lt;bean class=\"my.Service\"/&gt;      &lt;/jaxrs:serviceBeans&gt;      &lt;/jaxrs:server&gt;      &lt;/beans&gt;      </code></pre>      <p>So what my web app does is that creates all the required <code>.class</code> files for a new Web Service and then it adds a <code>&lt;bean class=\"new.Service2\"/&gt;</code> in the <code>&lt;jaxrs:serviceBeans&gt;</code> tag by editing the <code>beans.xml</code> file. </p>      <p>My problem is that the new service is not working if I don't restart tomcat which is a bit of an issue. Is there any way to make changes in the <code>&lt;jaxrs:serviceBeans&gt;</code> tag to be picked up at runtime without the need to restart tomcat? </p>      <p>Thanks </p>",
            tags: "<spring><web-services><rest><tomcat><cxf>"
            );

            posts.Add(
            id: 18629585,
            title: "how to execute REST request one by one",
            parentId: null,
            acceptedAnswerId: null,
            creationDate: DateTime.Parse("2013-09-05T06:50:39"),
            body: "<p>i get response of \"request 1\" , but when request 2 is executed i get Timeout Error      i have tried to execute \"request 2\" and then executing \"request 1\" , this time i successfully get Response of both the requests.      this is the code :</p>      <pre><code>var client = new RestClient();      client.BaseUrl = \"http://api.******.com/login/hostLogin\";      client.Authenticator = new DigestAuthenticator(\"****\", \"123456\");      var request = new RestRequest();      request.AddParameter(\"username\",txtlogin.Text);      IRestResponse response = client.Execute(request);      </code></pre>      <p>do i need to do something like DISPOSE or CLEARHANDLERS()      please have a look at my problem and tell me if i need to give more information regarding this.</p>",
            tags: "<c#><winforms><rest><httpwebrequest><restsharp>"
            );

            posts.Add(
            id: 18621756,
            title: "PUT or POST when making idempotnet request for specific use-case",
            parentId: null,
            acceptedAnswerId: null,
            creationDate: DateTime.Parse("2013-09-04T19:01:50"),
            body: "<p>I'm designing REST API for web application.</p>      <p>My principles while designing API are:</p>      <ul>      <li>Use client <em>use cases</em> perspective rather then data model perspective. Motivation: abstract from DB schema.</li>      <li>Each slash represent new use case/action.</li>      </ul>      <p>Lets say that In the application we have users, products, locations, product-news. Use case is <em>user follows product-news from some location</em>. If location is empty then user follows news about product from each location.<br>      The list of products and locations is well known.</p>      <h3>What is the right method for adding user as a follower of a specific product-location combination?</h3>      <p>I end up with the following URL:</p>      <pre><code>/product/follow?product=&lt;product_name&gt;[&amp;location=&lt;location name&gt;]      </code></pre>      <p>The <code>product</code> and <code>location</code> names are in query part because is more flexible to extend in future.</p>      <ul>      <li>The argument for <strong>PUT</strong>: Of course this request is idempotent - sending it multiple times doesn't make any other change as sending it once.</li>      <li>The argument for <strong>POST</strong>: we don't specify URL, under which the resource is set.</li>      </ul>      <p>Personally I'm closer to <strong>PUT</strong> because by the <em>use-case</em> principle for API (which I consider as the right one) the <em>impotency</em> rule seams to be the most corresponding one.</p>",
            tags: "<api><http><rest><design><restful-url>"
            );

            posts.Add(
            id: 18619649,
            title: "In Jersey, how do you deal with @POST parameters of a deeply nested, complex object?",
            parentId: null,
            acceptedAnswerId: null,
            creationDate: DateTime.Parse("2013-09-04T16:54:39"),
            body: "<p>I'm using Jersey 1.x here and I have a <code>@POST</code> method that requires sending over a deeply nested, complex object.  I'm not sure of all my options, but it seems like a lot are <a href=\"http://jersey.java.net/nonav/documentation/1.8/user-guide.html#d4e253\" rel=\"nofollow\">described in this documentation</a>: </p>      <blockquote>      <p>In general the Java type of the method parameter may:</p>      <ol>      <li><p>Be a primitive type;</p></li>      <li><p>Have a constructor that accepts a single String argument;</p></li>      <li><p>Have a static method named valueOf or fromString that accepts a single String argument (see, for example, Integer.valueOf(String) and      java.util.UUID.fromString(String)); or</p></li>      <li><p>Be List, Set or SortedSet, where T satisfies 2 or 3 above. The resulting collection is read-only.</p></li>      </ol>      </blockquote>      <p>Ideally, I wish that I could define a method like this:</p>      <pre><code>@POST      @Consumes(MediaType.APPLICATION_FORM_URLENCODED)      @Path(\"complexObject\")      public void complexObject(@FormParam(\"complexObject\") ComplexObject complexObject) throws Exception {      </code></pre>      <p>But I guess I can only do that if my object satisfies the requirements above (which in my case, it does not).  To me it seems that I have a choice.  </p>      <h3>Option 1: Implement <code>fromString</code></h3>      <p>Implement item #3 above.  </p>      <h3>Option 2: Pass in the <code>complexObject</code> in pieces</h3>      <p>Break up the <code>complexObject</code> into pieces so the parameters become this:</p>      <pre><code>@POST      @Consumes(MediaType.APPLICATION_FORM_URLENCODED)      @Path(\"complexObject\")      public void complexObject(@FormParam(\"piece1\") LessComplexPiece lessComplexPiece1,      @FormParam(\"piece2\") LessComplexPiece lessComplexPiece2,      @FormParam(\"piece3\") LessComplexPiece lessComplexPiece3) throws Exception {      </code></pre>      <p>This may not be enough if <code>LessComplexPiece</code> does not satisfy the requirements above.  I'm wondering what the best option is here.  What do people usually do in this situation?  Here are the pros and cons I can think of:</p>      <h1>Cons of Implement <code>fromString</code></h1>      <ul>      <li>Have to maintain a custom deserializer.  Every time the class is modified, this deserializer may break.  There's more risk for bugs in general.</li>      <li>It will probably be impossible to generate documentation that describes the pieces of the complex object.  I'll have to write that by hand.</li>      <li>For each piece of the complex object, I'll have to write my own casting and validation logic.</li>      <li>I'm not sure what the post data would look like.  But, this may make it very difficult for someone to call the API from a web page form.  If the resource accepted primitives, it would be easy. EG: <code>complexObject=seralizedString</code> vs <code>firstName=John</code> and <code>lastName=Smith</code></li>      <li>You may not be able to modify the class for various reasons (thankfully, this is not a limitation for me)</li>      </ul>      <h1>Pros of Implementing <code>fromString</code></h1>      <ul>      <li>This could avoid a method with a ton of parameters.  This will make the API less intimidating to use.</li>      <li>This argument is at the level of abstraction I want to work at in the body of my method:</li>      <li>I won't have to combine the pieces together by hand (well technically I will, it'll just have to be in the deserializer method)</li>      <li>The deserializer can be a library that automates the process (XStream, gensen, etc.) and save me a lot of time.  This can mitigate the bug risk.  </li>      <li>You may run into \"namespace\" clashes if you flatten the object to send over pieces.  For example, imagine sending over an <code>Employee</code>.  If he has a <code>Boss</code>, you now have to provide a <code>EmployeeFirstName</code> and a <code>BossFirstName</code>.  If you were just deserializing an object, you could nest the data appropriately and not have to include context in your parameter names.  </li>      </ul>      <p>So which option should I choose?  Is there a 3rd option I'm not aware of?</p>",
            tags: "<java><rest><jersey><jersey-1.0>"
            );

            posts.Add(
            id: 18618745,
            title: "Web API POST method returns HTTP/1.1 500 Internal Server Error",
            parentId: null,
            acceptedAnswerId: null,
            creationDate: DateTime.Parse("2013-09-04T16:05:27"),
            body: "<p>As the title says i have got a 500 internal server error when using post method of a Web API. The Get method is working fine, just getting error in POST.</p>      <p>I am using fidler to send post request : </p>      <p><strong>Response Header:</strong>      HTTP/1.1 500 Internal Server Error</p>      <p><strong>Request Header:</strong>      User-Agent: Fiddler      Host: localhost:45379      Content-Type: application/jsonContent-Length: 41      Content-Length: 41</p>      <p><strong>Request Body:</strong>      {\"iduser\"=\"123456789\",\"username\"=\"orange\"}</p>      <p><strong>Here is my code for post method:</strong></p>      <pre><code>     // POST api/User      public HttpResponseMessage Postuser(user user)      {      if (ModelState.IsValid)      {      db.users.Add(user);      db.SaveChanges();      HttpResponseMessage response =R  equest.CreateResponse(HttpStatusCode.Created, user);      response.Headers.Location = new Uri(Url.Link(\"DefaultApi\", new { id = user.iduser }));      return response;      }      else      {      return Request.CreateResponse(HttpStatusCode.BadRequest);      }      }      </code></pre>      <p><strong>Sooooooo what could have possibly gone wrong? Why its not allowing me to POST?</strong></p>",
            tags: "<web-services><rest><asp.net-mvc-4><asp.net-web-api><web-api>"
            );

            posts.Add(
            id: 18611419,
            title: "Getting 405 &quot;Method Not Allowed&quot; error using POST with @FormParam (Java web service with Jersey REST)",
            parentId: null,
            acceptedAnswerId: 18693474,
            creationDate: DateTime.Parse("2013-09-04T10:31:29"),
            body: "<p>I know it is not easy to pass something to the REST Server (Resource) which is neither String nor a simple Type. <br>      But for a simple ordering process I need to send a list of articles (which shall be ordered) from the client to teh server. </p>      <p>I already tried using \"QueryParam\", converting my object (I wrapped the list into a DTO) into JSON-String and passing it. It didn't work. (But for other methods which don't need to pass an object to the server my service works fine, even POST methods.)</p>      <p>Then I found out about the @FormParam which can theoretically transfer every kind of object. (That's what I read, is it actually true?)</p>      <p>So I tried in a very simple test method to pass a List of Strings to the Service, the serverside should give back the number of elements of that list.</p>      <p>That's my code:</p>      <p>On Server-Side (Resource):</p>      <pre><code>@Path(\"bestellung\")      public class BestellungResource {      @Path(\"test\")      @POST      @Consumes(MediaType.APPLICATION_FORM_URLENCODED)      @Produces(XML)      public Integer test(      @FormParam(\"list\") List&lt;String&gt; list){      return list.size();      }      }      </code></pre>      <p><br></p>      <p>And on Client Side (in a Session Bean):</p>      <pre><code>public Integer  test() {      List&lt;String&gt; list = new ArrayList&lt;String&gt;();      list.add(\"1\");      list.add(\"2\");      list.add(\"3\");      Form form = new Form();      form.add(\"list\", list);      return service      .path(\"bestellung\")      .path(\"test\")      .type(MediaType.APPLICATION_FORM_URLENCODED)      .post(Integer.class, form);      }      </code></pre>      <p>Where <strong>service</strong> is built like that:</p>      <pre><code>ClientConfig config = new DefaultClientConfig();      Client client = Client.create(config);      service = client.resource(UriBuilder.fromUri(\"&lt;service url&gt;\").build());      </code></pre>      <p>Invoking this client method from my GUI or directly via EJB Explorer always gives the 405 error.</p>      <p>Where's the problem? Did I miss something with POST, the MIME types or the Form?</p>      <p>By the way, even with simple form parameters like a String or int it does not work and throws a 405 error as well.</p>      <p>Thanks for your help!<br>      Jana</p>",
            tags: "<java><rest><post><jersey><form-parameter>"
            );

            posts.Add(
            id: 18610041,
            title: "Failed to invoke the service. Possible causes: The service is offline or inaccessible while Invoking",
            parentId: null,
            acceptedAnswerId: null,
            creationDate: DateTime.Parse("2013-09-04T09:25:06"),
            body: "<p>I have my service configured as below:</p>      <pre><code>&lt;system.serviceModel&gt;      &lt;services&gt;      &lt;service name=\"WcfService1.Service1\" behaviorConfiguration=\"MyServiceTypeBehaviors\"&gt;      &lt;host&gt;      &lt;baseAddresses&gt;      &lt;add baseAddress=\"net.tcp://127.0.0.1:808/service\" /&gt;      &lt;/baseAddresses&gt;      &lt;/host&gt;      &lt;endpoint address=\"net.tcp://127.0.0.1:808/service/\"      binding=\"netTcpBinding\"      contract=\"WcfService1.IService1\"/&gt;      &lt;endpoint address=\"mex\" binding=\"mexTcpBinding\" contract=\"IMetadataExchange\"   /&gt;      &lt;/service&gt;      &lt;/services&gt;      &lt;protocolMapping&gt;      &lt;add scheme=\"net.tcp\" binding=\"netTcpBinding\"/&gt;      &lt;/protocolMapping&gt;      &lt;behaviors&gt;      &lt;serviceBehaviors&gt;      &lt;behavior name=\"MyServiceTypeBehaviors\"&gt;      &lt;serviceMetadata httpGetEnabled=\"true\"/&gt;      &lt;/behavior&gt;      &lt;/serviceBehaviors&gt;      &lt;endpointBehaviors&gt;      &lt;behavior name=\"MyEndpointBehaviour\"&gt;      &lt;webHttp/&gt;      &lt;/behavior&gt;      &lt;/endpointBehaviors&gt;      &lt;/behaviors&gt;      &lt;serviceHostingEnvironment aspNetCompatibilityEnabled=\"true\" multipleSiteBindingsEnabled=\"true\" /&gt;      &lt;/system.serviceModel&gt;      </code></pre>      <p>and the client as:</p>      <pre><code> &lt;system.serviceModel&gt;      &lt;bindings&gt;      &lt;netTcpBinding&gt;      &lt;binding name=\"NetTcpBinding_IService1\" sendTimeout=\"00:05:00\" /&gt;      &lt;/netTcpBinding&gt;      &lt;/bindings&gt;      &lt;client&gt;      &lt;endpoint address=\"net.tcp://127.0.0.1/service/\" binding=\"netTcpBinding\"      bindingConfiguration=\"NetTcpBinding_IService1\" contract=\"IService1\"      name=\"NetTcpBinding_IService1\"&gt;      &lt;identity&gt;      &lt;servicePrincipalName value=\"host/MachineName\" /&gt;      &lt;/identity&gt;      &lt;/endpoint&gt;      &lt;/client&gt;      &lt;/system.serviceModel&gt;      </code></pre>      <p>When using WCFTestClient or SVCutil, I am able to discover and access the servie and generate proxy or stubs.</p>      <p>But when I want to invoke any of the methods getting following error:</p>      <blockquote>      <p>Failed to invoke the service. Possible causes: The <strong>service is offline</strong> or <strong>inaccessible</strong>; the <strong>client-side configuration does not match the proxy</strong>; the existing proxy is invalid. Refer to the stack trace for more detail. You can try to recover by starting a new proxy, restoring to default configuration, or refreshing the service.</p>      <p><strong>The socket connection was aborted.</strong> This could be caused by an error processing your message or a receive timeout being exceeded by the remote host, or an underlying network resource issue. Local socket timeout was '00:04:59.9980468'.</p>      </blockquote>",
            tags: "<c#><.net><wcf><rest>"
            );

            posts.Add(
            id: 18610033,
            title: "inner join unrelated table / entities on odata wcf data service",
            parentId: null,
            acceptedAnswerId: null,
            creationDate: DateTime.Parse("2013-09-04T09:24:35"),
            body: "<p>I have a question related to Odata in WCF data Service</p>      <p>I have 2 tables with no relationship in SQL Server :</p>      <pre><code>CREATE TABLE [dbo].[tblCity](      [CitID] [smallint] IDENTITY(1,1) NOT NULL,      [citName] [varchar](30) NOT NULL,      CONSTRAINT [PK_tblCity] PRIMARY KEY CLUSTERED      (      [CitID] ASC      )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]      ) ON [PRIMARY]      GO      -----------------------------------------------------------------------      CREATE TABLE [dbo].[tblVendor](      [VndID] [int] IDENTITY(1,1) NOT NULL,      [CitID] [smallint] NOT NULL,      CONSTRAINT [PK_tblVendor] PRIMARY KEY CLUSTERED      (      [VndID] ASC      )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]      ) ON [PRIMARY]      GO      </code></pre>      <p>And I'm <strong>not allowed</strong> to create relationship between those 2 tables, neither in the database nor in the EF.</p>      <p>I tried using this :</p>      <pre><code>http://localhost:15122/testWcfDataService.svc/tblVendor?$expand=CitID/tblCity&amp;filter=tblCity/citName%20eq%20'BATAM'&amp;$format=json&amp;$top=10&amp;$select=CitID/tblCity      </code></pre>      <p>But, there's error : </p>      <blockquote>      <pre><code>Inner or start path segments must be navigation properties in $select.      </code></pre>      </blockquote>      <p>However, using this is still ok :</p>      <pre><code>http://localhost:15122/testWcfDataService.svc/tblVendor?$expand=CitID/tblCity&amp;filter=tblCity/citName%20eq%20'BATAM'&amp;$format=json&amp;$top=10      </code></pre>      <p>So, my guess is the problem on the <strong>$select</strong></p>      <p>So, what's the solution to my problem?</p>      <p><strong>Note :</strong> this also happening to a lot of tables (hundreds of them), so I'm avoiding the stored procedure or view, at least for now</p>      <p><strong>Updated</strong></p>      <p>I'm using standard WCF Data Service with Entity Data Framework 5</p>      <p>My code is like this :</p>      <pre><code>public class testWcfDataService : DataService&lt;SOFIXPBE_2012Entities&gt;      {      // This method is called only once to initialize service-wide policies.      public static void InitializeService(DataServiceConfiguration config)      {      // TODO: set rules to indicate which entity sets and service operations are visible, updatable, etc.      // Examples:      config.UseVerboseErrors = true;      config.SetEntitySetAccessRule(\"*\", EntitySetRights.All);      config.SetServiceOperationAccessRule(\"*\", ServiceOperationRights.All);      config.SetEntitySetPageSize(\"*\", 10);      config.DataServiceBehavior.MaxProtocolVersion = DataServiceProtocolVersion.V3;      }      }      </code></pre>      <p>There are a few method, but I'm sure it has nothing to do with the problem</p>",
            tags: "<entity-framework><wcf-data-services><odata>"
            );

            posts.Add(
            id: 18610009,
            title: "Emberjs template not binding to restful data",
            parentId: null,
            acceptedAnswerId: 18616775,
            creationDate: DateTime.Parse("2013-09-04T09:23:10"),
            body: "<p>Got this route to get the data from a restful service</p>      <pre><code>var App = Ember.Application.create({rootElement: '#planner'});      App.Store = DS.Store.extend();      App.Router.map(function(){      this.resource('home');      });      App.HomeRoute = Ember.Route.extend({      model: function(){      return Ember.$.getJSON('/api/get-planner/');      }      });      </code></pre>      <p>And template:</p>      <pre><code>&lt;script type=\"text/x-handlebars\" data-template-name=\"home\"&gt;      {{name}}      &lt;/script&gt;      </code></pre>      <p>Somehow the value of name is not displayed. I can confirm the api is returning correct json data.</p>",
            tags: "<templates><rest><data><binding><ember.js>"
            );

            posts.Add(
            id: 18608051,
            title: "What is difference between httpMethod.releaseConnection() v/s EntityUtils.consume()?",
            parentId: null,
            acceptedAnswerId: null,
            creationDate: DateTime.Parse("2013-09-04T07:38:57"),
            body: "<p>We are trying to make connection multiple connection using <strong>MultiThreadedHttpConnectionManager</strong> of common-httpCliient-1.3.jar library. We are trying to jets3t to handle all these connections.</p>      <p>Environment to run sample application is -      max-connections = 20, Operating System - Windows 7</p>      <p>Sometime it produce following exception  -</p>      <blockquote>      <p>java.net.BindException: Address already in use: connectStacktrace      org.jets3t.service.i: Request Error: java.net.BindException: Address      already in use: connect GET '/' at      org.jets3t.service.impl.rest.a.h.a(Unknown Source) at      org.jets3t.service.impl.rest.a.h.a(Unknown Source) at      org.jets3t.service.impl.rest.a.h.O(Unknown Source) at      org.jets3t.service.k.L(Unknown Source) at      org.jets3t.service.impl.rest.a.h.M(Unknown Source) at      org.jets3t.service.k.J(Unknown Source) at      com.my.main.sg.run(Unknown Source) Caused by:      java.net.BindException: Address already in use: connect at      java.net.TwoStacksPlainSocketImpl.socketConnect(Native Method) at      java.net.AbstractPlainSocketImpl.doConnect(AbstractPlainSocketImpl.java:339)      at      java.net.AbstractPlainSocketImpl.connectToAddress(AbstractPlainSocketImpl.java:200)      at      java.net.AbstractPlainSocketImpl.connect(AbstractPlainSocketImpl.java:182)</p>      </blockquote>      <p>We have checked everywhere it is creating connection via MultiThreadedHttpConnectionManager and there is proper use of <strong><code>httpMethod.releaseConnection()</code></strong>.</p>      <p>Now I have compared new code of jets3t-0.9.0 with 0.8.1, there are changes of httpClient-4.1.2.jar instead of common-httpClient lib. Here connection if released via <strong><code>EntityUtils.consume(response.getEntity())</code></strong></p>      <p>Can this binding issue be solved by using EntityUtils.consume() method of httpClient library solve my issue of <code>java.net.BindException: Address already in use: connect</code> . Not only we are getting these exception in GET but in PUT also we are getting.</p>      <p>What are the difference between these to methods of releasing connections EntityUtils.consume(response.getEntity()) and httpMethod.releaseConnection()?</p>      <p>Thanks in advance for your help,</p>      <p>Neelam Sharma</p>",
            tags: "<java><apache><rest><httpclient><apache-commons>"
            );

            posts.Add(
            id: 18604864,
            title: "rest api that allows update to collection",
            parentId: null,
            acceptedAnswerId: null,
            creationDate: DateTime.Parse("2013-09-04T03:20:02"),
            body: "<p>what would be the best practice for allowing a WebAPI OData based webservice update an entire collection?  </p>      <p>For example, we have an admin page that allows users to maintain a list of payment terms.  We have created a controller that is based on the PaymentTerm entity, which allows the standard Get, Get by key, Put, Post, and Delete, for working with single instances of the PaymentTerm Entity.  However, our UI team would like to retrieve a collection of payment terms (easily done with the standard Get collection), manipulate it locally, and then Put or Post the entire collection back to the server, rather than having to make a series of Put, Post, and Delete calls to the server.  </p>      <p>I have tried creating an action method for this, and while I have managed to get it to work, it seems somewhat kludgy, as it requires an ID, as well as odata parameters (which contain the collection), and the ID is meaningless because at this point you are not working with an instance of a Payment Term, but an entire collection of them.</p>      <p>I could create a new controller solely for working with a collection of payment terms, but I'm not sure that would be much better, as it would end up having to have a base class declaration of </p>      <pre><code>EntitySetController&lt;PaymentTermCollection, int&gt;      </code></pre>      <p>or the like, which would not make much sense as the collection would not have a key that had any meaning.  </p>",
            tags: "<asp.net-web-api><odata><web-api>"
            );

            posts.Add(
            id: 18603761,
            title: "Choosing the right OAuth2 grant type for PHP web app",
            parentId: null,
            acceptedAnswerId: 18619883,
            creationDate: DateTime.Parse("2013-09-04T00:55:55"),
            body: "<p>I'm building a very typical web app product. It will likely have corresponding mobile apps in the future. I'm building it from the ground up with a REST API, which is secured using OAuth2. I've got OAuth2 working, and I'm able to connect successfully using various grant types.</p>      <p>What I'm a little confused about is what grant types to use for the actual web app. Here's what I had in mind:</p>      <h2>Public API access</h2>      <p>Before a user logs into the web app, some API access is required for things like user registration and password resets. I was thinking of using the <code>client_credientials</code> grant type. A simple client id and secret validation in return for an access token.</p>      <p>However, it seems totally unnecessary to request an access to token for every single public request or even for each session. It seems to make more sense to just generate ONE access token that my web app will always use.</p>      <p>Yet, this seems to go against how OAuth is designed to work. For example, access tokens expire. What is the right way of doing this?</p>      <h2>Private user API access</h2>      <p>Next, for a user to login to the web app I was planning on using the <code>password</code> grant type (resource owner password credentials). This approach allows me to save the <code>user_id</code> with the access token—so I know which user is logged in. Further, by using scopes I can restrict access within the API.</p>      <p>I plan to save the access token within the PHP session. As long as the PHP session is active they will remain logged into the web app.</p>      <p>Is this an appropriate design for user login?</p>",
            tags: "<php><rest><oauth-2.0>"
            );

            posts.Add(
            id: 18602066,
            title: "Creating a Mockup REST API",
            parentId: null,
            acceptedAnswerId: 18602229,
            creationDate: DateTime.Parse("2013-09-03T21:50:01"),
            body: "<p>I'm currently trying to create a NodeJS server or something similar to mockup a REST API that reads in a JSON file and responds to requests with that data. I really only need GET requests supported. What is the best way to go about this?</p>      <p>Here's what I have so far:</p>      <pre><code>/**      * Sample items REST API      */      function ItemsRepository() {      this.items = [];      }      ItemsRepository.prototype.find = function (id) {      var item = this.items.filter(function(item) {      return item.itemId == id;      })[0];      if (null == item) {      throw new Error('item not found');      }      return item;      }      /**      * Retrieve all items      * items: array of items      */      ItemsRepository.prototype.findAll = function () {      return this.items;      }      /**      * API      */      var express = require('express');      var app = express();      var itemRepository = new ItemsRepository();      app.configure(function () {      // used to parse JSON object given in the body request      app.use(express.bodyParser());      });      /**      * HTTP GET /items      * items: the list of items in JSON format      */      app.get('/items', function (request, response) {      response.json({items: itemRepository.findAll()});      });      /**      * HTTP GET /items/:id      * Param: :id is the unique identifier of the item you want to retrieve      * items: the item with the specified :id in a JSON format      * Error: 404 HTTP code if the item doesn't exists      */      app.get('/items/:id', function (request, response) {      var itemId = request.params.id;      try {      response.json(itemRepository.find(itemId));      } catch (exception) {      response.send(404);      }      });      app.listen(8080); //to port on which the express server listen      </code></pre>      <p>I know that I would use the following to include the file, I just don't know how to stuff the data into Items.</p>      <pre><code>var responseItemsData = require('./items-list.json');      </code></pre>",
            tags: "<javascript><json><node.js><rest>"
            );

            posts.Add(
            id: 18600720,
            title: "getting header info ETaG of online txt file",
            parentId: null,
            acceptedAnswerId: null,
            creationDate: DateTime.Parse("2013-09-03T20:14:43"),
            body: "<p>I need to get header info ETaG of a txt url into a string. How to do that?</p>      <pre><code>String etag = con.getHeaderField(\"ETag\");      </code></pre>      <p>is not working for some reason. It does have the etag information in the header I checked.</p>      <p>I need to use this in an android app to check if file has been modified. Help needed urgently.</p>      <p><strong>Edit:</strong> Code that I am using:</p>      <pre><code>public void getFromOnlineTxtDatabase(){      try{      URL url = new URL(\"url-here\");      HttpURLConnection.setFollowRedirects(true);      HttpURLConnection con = (HttpURLConnection) url.openConnection();      con.setDoOutput(false);      con.setReadTimeout(20000);      con.setRequestProperty(\"Connection\", \"keep-alive\");      //get etag for update check      String etag = con.getHeaderField(\"etag\");      //String etag= \"\";      con.setRequestProperty(\"User-Agent\", \"Mozilla/5.0 (Windows NT 6.1; WOW64; rv:16.0) Gecko/20100101 Firefox/16.0\");      ((HttpURLConnection) con).setRequestMethod(\"GET\");      //System.out.println(con.getContentLength()) ;      con.setConnectTimeout(5000);      BufferedInputStream in = new BufferedInputStream(con.getInputStream());      int responseCode = con.getResponseCode();      if (responseCode == HttpURLConnection.HTTP_OK) {      System.out.println(responseCode);      }      StringBuffer buffer = new StringBuffer();      int chars_read;      //int total = 0;      while ((chars_read = in.read()) != -1)      {      char g = (char) chars_read;      buffer.append(g);      }      final String page = buffer.toString();      //create password_ems.txt to internal      if (fileExistance(\"data.txt\")){      File dir = getFilesDir();      File file = new File(dir, \"data.txt\");      boolean deleted = file.delete();      stringToTxt(page, \"data.txt\");      }else{      stringToTxt(page, \"data.txt\");      }      if (fileExistance(\"data_etag.txt\")){      File dir = getFilesDir();      File file = new File(dir, \"etag.txt\");      boolean deleted = file.delete();      stringToTxt(etag, \"etag.txt\");      }else{      //create etag_file      stringToTxt(etag, \"data_etag.txt\");      }      //  Log.i(\"Page\", page);      }catch(Exception e){      showDialog(\"Database Fetch Failure\",\"Unable to Fetch Password Database, check your internet\" +      \" connection and try again later.\",0);      Log.i(\"Page\", \"Error\");      }      }      </code></pre>",
            tags: "<java><android><html><rest><http-headers>"
            );

            posts.Add(
            id: 18596156,
            title: "PHP send a REST message within body and not as multipart",
            parentId: null,
            acceptedAnswerId: 18652688,
            creationDate: DateTime.Parse("2013-09-03T15:33:54"),
            body: "<p>we have an issue with a PHP client sending a REST / POST message. We receive the message payload as an attachment, while we would expect it in the message content itself.</p>      <pre><code>    &lt;http:Accept&gt;*/*&lt;/http:Accept&gt;      &lt;http:Connection&gt;close&lt;/http:Connection&gt;      &lt;http:Content-Length&gt;385&lt;/http:Content-Length&gt;      &lt;http:Content-Type&gt;multipart/form-data; boundary=----------------------------41eae3cb899c&lt;/http:Content-Type&gt;      &lt;http:Host&gt;JIT&lt;/http:Host&gt;      &lt;/tran:headers&gt;      </code></pre>      <p>While if I send the request using curl like</p>      <pre><code>  curl -H \"username:XXX\"  -X POST -d '&lt;message /&gt;' http://myurl.com      </code></pre>      <p>it works properly, what should I ask to check?</p>",
            tags: "<php><rest>"
            );

            posts.Add(
            id: 18594366,
            title: "Sinatra with sessions using Rack::Session::Pool",
            parentId: null,
            acceptedAnswerId: 0,
            creationDate: DateTime.Parse("2013-09-03T14:04:28"),
            body: "<p>I am having a problem with Sinatra using <code>Rack::Session::Pool</code> for storing session information.      What I want to do is to 'post' some data by appending information to the session hash using <code>'POST /dataset'</code>, and then retrieve it by using <code>'GET /dataset'</code> and returning the content of 'session' hash. When I try to return the value though, the 'session' hash does not contain the :message key</p>      <pre><code>require 'sinatra/base'      class Trial &lt; Sinatra::Base      use Rack::Session::Pool      post '/dataset' do      session[:message] = params[:data]      end      get '/dataset' do      session[:message]      end      end      Trial.run!      </code></pre>      <p>I know this looks trivial, but still I can't get it to work...</p>",
            tags: "<ruby><rest><session><sinatra><rack>"
            );

            posts.Add(
            id: 18594270,
            title: "ExpressJS &amp; Mongoose REST API structure: best practices?",
            parentId: null,
            acceptedAnswerId: 18597200,
            creationDate: DateTime.Parse("2013-09-03T13:59:57"),
            body: "<p>I'm building a REST API with the use of NodeJS (Mongoose &amp; ExpressJS). I think I have a pretty  good basic structure at the moment, but I'm wondering what the best practices are for this kind of project.</p>      <p>In this basic version, everything passes through the <code>app.js</code> file. Every HTTP method is then passed to the resource that has been requested. This allows me to dynamically add resources to the API and every request will be passed along accordingly. To illustrate:</p>      <pre><code>// app.js      var express = require('express');      var mongoose = require('mongoose');      var app = express();      app.use(express.bodyParser());      mongoose.connect('mongodb://localhost/kittens');      var db = mongoose.connection;      var resources = [      'kitten'      ];      var repositories = {};      for (var i = 0; i &lt; resources.length; i++) {      var resource = resources[i];      repositories[resource] = require('./api/' + resource);      }      db.on('error', console.error.bind(console, 'connection error:'));      db.once('open', function callback() {      console.log('Successfully connected to MongoDB.');      app.get('/:resource', function (req, res) {      res.type('application/json');      repositories[req.params.resource].findAll(res);      });      app.get('/:resource/:id', function (req, res) {      res.type('application/json');      repositories[req.params.resource].findOne(req, res);      });      app.listen(process.env.PORT || 4730);      });      </code></pre>      <p>-</p>      <pre><code>// api/kitten.js      var mongoose = require('mongoose');      var kittenSchema = mongoose.Schema({      name: String      });      var Kitten = mongoose.model('Kitten', kittenSchema);      exports.findAll = function (res) {      Kitten.find(function (err, kittens) {      if (err) {      }      res.json(kittens);      });      };      exports.findOne = function (req, res) {      Kitten.findOne({ _id: req.params.id}, function (err, kitten) {      if (err) {      }      res.json(kitten);      });      };      </code></pre>      <p>Obviously, only a couple of methods have been implemented so far. What do you guys think of this approach? Anything I could improve on?</p>      <p>Also, a small side question: I have to require mongoose in every API resource file (like in <code>api\\kitten.js</code>, is there a way to just globally require it in the app.js file or something?</p>      <p>Any input is greatly appreciated!</p>",
            tags: "<node.js><mongodb><rest><express><mongoose>"
            );

            posts.Add(
            id: 18592058,
            title: "Strategy to have deleted objects in DB deleted in Core Data as well. [RestKit/Core Data]",
            parentId: null,
            acceptedAnswerId: 18744730,
            creationDate: DateTime.Parse("2013-09-03T12:12:58"),
            body: "<p>I have a REST service running on top of my application, which returns data to my iPad app. This app is built using RestKit to sync data in and out of the iPad. I have however a webapp running as well, which allows the users to delete some data.</p>      <p>The current problem that I have right now, is that whenever a user logs in into the iPad app, I run a query to get the data that was last_modified/added since his last login. This allows me to have faster/shorter queries. The only problem, is that if for example an object was deleted from the DB between his last two logins, the user will still see it in his iPad.</p>      <p>What strategy should I adopt to have this data in Core Data deleted as well? Should I just <strong>not</strong> delete object from my DB and have instead a BOOL that says \"deleted\" or not, and whenever I get the last_modified data via REST, this item will appear and I will just filter it out in the iPad?</p>      <p>I know RestKit has a way to delete orphans objects, but since I am syncing the \"last_modified\" data, I don't think it can be applied here.</p>",
            tags: "<ios><rest><core-data><restkit><restkit-0.20>"
            );

            posts.Add(
            id: 18591883,
            title: "How to create more than one objects of different classes in Tastypie?",
            parentId: null,
            acceptedAnswerId: null,
            creationDate: DateTime.Parse("2013-09-03T12:02:41"),
            body: "<p>I've just started working on tastypie and I'm stuck with this problem. I have 2 model classes, </p>      <pre><code>class Quiz(models.Model):      name = models.CharField()      </code></pre>      <p>class Question(models.Model):</p>      <pre><code>qstn = models.CharField()      quiz = models.ForeignKey(Quiz)      ...      </code></pre>      <p>I'm stuck with trying to send a single post which would create a Quiz object and question objects.      Is it possible to do so?</p>",
            tags: "<django><rest><tastypie>"
            );

            posts.Add(
            id: 18666367,
            title: "How to pass List to Spring Batch ItemReader via REST call",
            parentId: null,
            acceptedAnswerId: null,
            creationDate: DateTime.Parse("2013-09-06T20:49:36"),
            body: "<p>I'm working on a REST method that will perform a job using Spring Batch.</p>      <p>I have a simple job defined,</p>      <pre><code>&lt;job id=\"myIndexJob\" xmlns=\"http://www.springframework.org/schema/batch\"&gt;      &lt;step id=\"step1\"&gt;      &lt;tasklet&gt;      &lt;chunk reader=\"myIndexItemReader\" processor=\"myIndexItemProcessor\" writer=\"myIndexItemWriter\" commit-interval=\"1\" /&gt;      &lt;/tasklet&gt;      &lt;/step&gt;      &lt;/job&gt;      </code></pre>      <p>This job mimics a question I posted earlier,</p>      <p><a href=\"http://stackoverflow.com/questions/18519645/spring-batch-itemreader-list-processed-only-once\">Spring Batch ItemReader list processed only once</a></p>      <p>But this time, instead of executing the job on a schedule, I want to manually execute it via a REST call.</p>      <p>The problem I'm having is passing a <code>List</code> to the <code>myIndexItemReader</code>. My REST call will generate a <code>List</code> based on some query string. The previous question I posted got it's <code>List</code> handed to it via the spring bean in the XML each time the step ran.</p>      <p>I'd like to do something like this,</p>      <pre><code>@RequestMapping(value=\"/rest/{regex}\", method=RequestMethod.GET)      public void run(@PathVariable String regex) {      List&lt;String&gt; myList = new ArrayList&lt;&gt;();      myList.add(\"something\");      long nanoBits = System.nanoTime() % 1000000L;      if (nanoBits &lt; 0) {      nanoBits *= -1;      }      String dateParam = new Date().toString() + System.currentTimeMillis()      + \".\" + nanoBits;      JobParameters param = new JobParametersBuilder()      .addString(\"date\", dateParam)      .toJobParameters();      JobExecution execution = jobLauncher.run(job, param);      }      </code></pre>      <p>but I just don't know how to pass <code>myList</code> to the <code>myIndexItemReader</code>.</p>      <p>As of now I can do this by creating a <code>RepeatTemplate</code> and calling <code>iterate</code> on a callback, but the job <code>chunk</code> seems more clean.</p>      <p>Anyone have any ideas or suggestions? Thanks /w</p>",
            tags: "<java><spring><rest><spring-batch>"
            );

            posts.Add(
            id: 18666307,
            title: "Encoding Body space as plus sign rest",
            parentId: null,
            acceptedAnswerId: null,
            creationDate: DateTime.Parse("2013-09-06T20:45:15"),
            body: "<p>I am trying to encode a body before sending to client.      I am using RestSharp.</p>      <p>This converted space as %20 which i want it to be + sign.</p>      <pre><code>var encodedBody = HttpUtility.UrlEncode(xmlRequest);      </code></pre>",
            tags: "<c#><rest><c#-4.0>"
            );

            posts.Add(
            id: 18666254,
            title: "Subtraction of two dates in OData query",
            parentId: null,
            acceptedAnswerId: null,
            creationDate: DateTime.Parse("2013-09-06T20:40:54"),
            body: "<p>I am exposing a table thru an OData feed based on WCF Data Services (version 5.6.0) with two dates in it - StartDate and EndDate. I would like to query my data for rows where e.g. the difference (TimeSpan) between the two dates is more than two hours.</p>      <p>Is this at all possible?</p>      <p>Naturally I could do it in T-SQL and also in LINQ, but I am missing the link (no pun intended) to doing it across the OData query layer.</p>      <p>Can you guys help? I'd much rather do it directly in the ODate query than defining a service operation to perform the task.</p>      <p>Thanks,</p>      <p>/Jesper</p>      <p>Copenhagen, Denmark</p>",
            tags: "<wcf-data-services><odata>"
            );

            posts.Add(
            id: 18662332,
            title: "Getting Item ID after REST Upload to SharePoint 2013 Online Document Library",
            parentId: null,
            acceptedAnswerId: null,
            creationDate: DateTime.Parse("2013-09-06T16:23:46"),
            body: "<p>Can somebody help me connect the dots between these functions. I can upload, but how do I get the ID of the file I just uploaded to update metadata columns on the File in the host Document Library?</p>      <p>Many Thanks!</p>      <pre><code>function uploadDocument(buffer, fileName) {      var url = String.format(\"{0}/_api/Web/Lists/getByTitle('Project Documents')/RootFolder/Files/Add(url='{1}', overwrite=true)\",      _spPageContextInfo.webAbsoluteUrl, fileName);      var call = jQuery.ajax({      url: url,      type: \"POST\",      data: buffer,      processData: false,      headers: {      Accept: \"application/json;odata=verbose\",      \"X-RequestDigest\": jQuery(\"#__REQUESTDIGEST\").val(),      \"Content-Length\": buffer.byteLength      }      });      return call;      }      function getItem(file) {      var call = jQuery.ajax({      url: file.ListItemAllFields.__deferred.uri,      type: \"GET\",      dataType: \"json\",      headers: {      Accept: \"application/json;odata=verbose\"      }      });      return call;      }      function updateItemFields(item) {      var now = new Date();      var call = jQuery.ajax({      url: _spPageContextInfo.webAbsoluteUrl +      \"/_api/Web/Lists/getByTitle('Project Documents')/Items(\" +      item.Id + \")\",      type: \"POST\",      data: JSON.stringify({      \"__metadata\": { type: \"SP.Data.Project_x0020_DocumentsItem\" },      CoordinatorId: _spPageContextInfo.userId,      Year: now.getFullYear()      }),      headers: {      Accept: \"application/json;odata=verbose\",      \"Content-Type\": \"application/json;odata=verbose\",      \"X-RequestDigest\": jQuery(\"#__REQUESTDIGEST\").val(),      \"IF-MATCH\": item.__metadata.etag,      \"X-Http-Method\": \"MERGE\"      }      });      return call;      }      </code></pre>",
            tags: "<jquery><json><rest><sharepoint2013><office365-apps>"
            );

            posts.Add(
            id: 18662097,
            title: "Parse API: Rest request for relation data",
            parentId: null,
            acceptedAnswerId: 18663258,
            creationDate: DateTime.Parse("2013-09-06T16:10:51"),
            body: "<p>I'm using the Parse API. I have a class, Category, that contains a field, subcategories. This field is a relation that points to other instances of the class Category. I want to perform a Rest request that retrieves the subcategories for a particular category, but I can't figure out how to make it work. Here's the Url I'm using right now:</p>      <pre><code>https://api.parse.com/1/classes/Category?where={\"$relatedTo\":{\"object\":{\"__type\":\"Pointer\",\"className\":\"Category\",\"objectID\":\"dkFGBAL5A7\"},\"key\":\"subcategories\"}}      </code></pre>      <p>The objectID is for the \"parent\" Category that I want to get subcategories for. However, I get the following error response:</p>      <pre><code>{      \"code\": 102,      \"error\": \"a valid pointer is needed for RelatedTo operator\"      }      </code></pre>      <p>I know that the objectID is correct. What am I doing wrong?</p>",
            tags: "<json><rest><parse.com>"
            );

            posts.Add(
            id: 18661910,
            title: "Debugging an HTTParty call using a REST client",
            parentId: null,
            acceptedAnswerId: null,
            creationDate: DateTime.Parse("2013-09-06T16:01:39"),
            body: "<p>We're using HTTParty to make web service calls, and I would like to know how to 'translate' the calls into something I can put into --for instance-- the <a href=\"https://chrome.google.com/webstore/detail/advanced-rest-client/hgmloofddffdnphfgcellkdfbfbjeloo?hl=en-US\" rel=\"nofollow\">Advanced Rest Client</a> plugin for Chrome.</p>      <p>Our HTTParty call looks like this:</p>      <pre><code>    HTTParty.post(      \"#{@url}#{@rest_method}\",      {      :headers =&gt; {'Authorization' =&gt; authorization_header,},      :query =&gt; query_hash,      :body =&gt; @body_params      }      )      </code></pre>      <p>...using the Advanced Rest Client I can create an Authorization header entry, and put the <code>authorization_header</code> value in there.  But what do I do with the <code>:query</code> and <code>:body</code> hashes?  Convert them to json and put them both in the payload section somehow?  If so how would I accomodate them both?  (I assume HTTParty converts those hashes to json strings before posting?)</p>",
            tags: "<ruby><rest><httparty>"
            );

            posts.Add(
            id: 18659265,
            title: "How to Consume Glassfish REST Interface",
            parentId: null,
            acceptedAnswerId: null,
            creationDate: DateTime.Parse("2013-09-06T13:40:28"),
            body: "<p>I am new to REST web services.  I would like to consume Glassfish REST interface (<code>http://localhost:4848/management/application.wadl</code> or <code>http://localhost:4848/management/domain/servers</code>) to stop/start a particular server instance.      I got a basic example and modified it as per my needs, but I am not able to do BASIC authentication.  Any guidance is appreciated.</p>      <pre><code>    HttpClient client = new HttpClient();      client.getState().setCredentials(      new AuthScope(\"http://127.0.0.1\", 4848, \"admin-realm\", \"BASIC\"),      new UsernamePasswordCredentials(\"admin\", \"admin123\"));      GetMethod method = new GetMethod(      \"http://localhost:4848/management/domain/servers\");      method.setDoAuthentication(true);      try {      int result = client.executeMethod(method);      System.out.println(\"Result Code = \" + result);      System.out.println(method.getResponseBodyAsString());      } catch (HttpException e) {      e.printStackTrace();      } catch (IOException e) {      e.printStackTrace();      }      </code></pre>      <p>ERROR</p>      <pre><code>2013-09-06 08:38:38,971 ERROR [org.apache.commons.httpclient.HttpMethodDirector] - Invalid challenge: BASIC      org.apache.commons.httpclient.auth.MalformedChallengeException: Invalid challenge: BASIC      at org.apache.commons.httpclient.auth.AuthChallengeParser.extractParams(AuthChallengeParser.java:98)      at org.apache.commons.httpclient.auth.RFC2617Scheme.processChallenge(RFC2617Scheme.java:94)      at org.apache.commons.httpclient.auth.BasicScheme.processChallenge(BasicScheme.java:112)      at org.apache.commons.httpclient.auth.AuthChallengeProcessor.processChallenge(AuthChallengeProcessor.java:162)      at org.apache.commons.httpclient.HttpMethodDirector.processWWWAuthChallenge(HttpMethodDirector.java:694)      at org.apache.commons.httpclient.HttpMethodDirector.processAuthenticationResponse(HttpMethodDirector.java:668)      at org.apache.commons.httpclient.HttpMethodDirector.executeMethod(HttpMethodDirector.java:193)      at org.apache.commons.httpclient.HttpClient.executeMethod(HttpClient.java:397)      at org.apache.commons.httpclient.HttpClient.executeMethod(HttpClient.java:323)      at com.car.service.UsedCarClient.main(UsedCarClient.java:42)      Result Code = 401      &lt;html&gt;&lt;head&gt;&lt;title&gt;GlassFish REST Interface&lt;/title&gt; &lt;link rel=\"stylesheet\" type=\"text/css\" href=\"static/std.css\" /&gt;&lt;/head&gt;&lt;body&gt;&lt;h1 class=\"mainheader\"&gt;GlassFish REST Interface&lt;/h1&gt;&lt;hr/&gt;&lt;/div&gt;&lt;/body&gt;&lt;/html&gt;      </code></pre>",
            tags: "<java><rest><glassfish>"
            );

            posts.Add(
            id: 18659102,
            title: "DAO using a ConcurrentHashMap",
            parentId: null,
            acceptedAnswerId: 18659516,
            creationDate: DateTime.Parse("2013-09-06T13:31:49"),
            body: "<p>Is it ok to have a REST Webservice (Spring+Jersey) that uses a DAO with a ConcurrentHashMap to store the data, or should I avoid it and use some kind of in-memory DB? </p>      <p>It's an sample application, so I don't mind losing the data every time the application stops.</p>",
            tags: "<java><spring><rest><dao><concurrenthashmap>"
            );

            posts.Add(
            id: 18656076,
            title: "Get json from Web Service",
            parentId: null,
            acceptedAnswerId: 18656182,
            creationDate: DateTime.Parse("2013-09-06T10:54:22"),
            body: "<p>I have a <code>REST</code> Web Service on Spring and now i want to <strong>split it into Server and Client</strong>.<br>      <strong>When everything was as one application it worked fine</strong>, but now i'm facing some issues.  </p>      <p>I don't  receive data from server, though i get <code>200 OK</code> from it.</p>      <p>Server-side (<code>http://localhost:8085</code>)</p>      <pre><code>@Controller      @RequestMapping(value = \"/user\")      public class RestController {      @RequestMapping(value = \"/{userLogin}\", method = RequestMethod.GET)      @ResponseBody      public Wrapper edit(@PathVariable String userLogin) {      return wrapper.wrap(userService.findByLogin(userLogin));      }      }      </code></pre>      <p>Client-side (<code>http://localhost:8089</code>)</p>      <pre><code>function editUser(login) {      $.ajax({      type: \"GET\",      url: \"http://localhost:8085/user/\" + login,      async: false,      success: function (resp) {      alert(\"asdasdasdasdasdas\");      }      });      }      </code></pre>      <p>When i manually access link <code>http://localhost:8085/user/user1</code> i see such line in browser</p>      <pre><code>{\"id\":1,\"login\":\"user1\",\"password\":\"user1\",\"passValid\":\"user1\",\"email\":\"user1@user1.nix\",\"firstname\":\"user1\",\"lastname\":\"user1\",\"birthday\":\"1940-08-10\",\"roleid\":\"User\"}      </code></pre>      <p>Could you please tell me what is wrong with my app?</p>",
            tags: "<java><jquery><json><web-services><rest>"
            );

            posts.Add(
            id: 18655691,
            title: "Error message when request includes User/Account comes only once",
            parentId: null,
            acceptedAnswerId: null,
            creationDate: DateTime.Parse("2013-09-06T10:30:41"),
            body: "<p>I'm sending the following request to SP: </p>      <pre><code>http://portal.at.magnaexin.int/0447/pro01sc01/_vti_bin/ListData.svc/Abwesenheitskalender?$filter=field1 eq null and field2 eq 'NO' and userfield/Account eq 'DOMAIN\\username'      </code></pre>      <p>and get this back (statuscode 400):</p>      <pre><code>&lt;message xml:lang=\"de-AT\"&gt;No property 'Account' exists in type 'Microsoft.SharePoint.Linq.DataServiceEntity' at position 58.&lt;/message&gt;      </code></pre>      <p>The second time I execute the request it works fine. Even when I copz this into the browser adressfield I get the same correct results.      Any ideas why this is happening?</p>",
            tags: "<ios><objective-c><rest><sharepoint>"
            );

            posts.Add(
            id: 18654128,
            title: "Context-Aware security with WCF and OData",
            parentId: null,
            acceptedAnswerId: null,
            creationDate: DateTime.Parse("2013-09-06T09:13:27"),
            body: "<p>I'm doing some research on this topic, and I'd really like some input on how to approach this. This is what I am doing:</p>      <p>I am running a WCF OData service which contains information about users, their resources, preferences etc. All of these connections between users and resources/preferences is assigned with a security attribute (let's say low, medium and high security).</p>      <p>My scenario is that I would like the application that talks to this service, receives information about the user and user relations based on context information (time, location, action and id). Basically calculating what parts of the user profile the application have access to (policy I guess) </p>      <p>There are several things holding me back. First of all, I have not found a way of returning allowed information back to the application without setting static entity access rules and restricting by using query interceptors or operations. What I am basically doing at the moment is to give AllRead access to entities and restricting data returned based on simple information in the HTTP header. It just seems wrong for me from a security point-of-view to give all access and then deny access to specific parts of the data</p>      <p>Another thing I would like to use is some kind of policy based access to the users data. Let's say that if the application possesses certain types of information about the user, it would be given an appropriate access policy and be given information associated with that policy.</p>      <p>I'm not looking for hundreds of lines of code or a proposed solution, just something that can point me in the right direction (or any direction).</p>      <p>I'm using EF, WCF5.0, OData 3.0.</p>      <p>Thanks</p>",
            tags: "<wcf><odata><access-control>"
            );

            posts.Add(
            id: 18653754,
            title: "WCF REST POST base64 encrypted string",
            parentId: null,
            acceptedAnswerId: null,
            creationDate: DateTime.Parse("2013-09-06T08:54:46"),
            body: "<p>I have a WCF REST service that is reciving (POST) a string of base64 encoded json data. When I test this in fiddler2 I get a <code>400 bad request error</code></p>      <p>The problem seems to be with the lenght of the posted string but I don't get it cos in the web config I set the buffers and stuff to values that should be enugh. I don't know mybe the fiddlers buffer not enough but this would be odd I guess</p>      <p>In my testing I found out that the service actually fires if the request is a bit shorter then the actual full encoded object So I guess it has someting to do with the buffer but I fail to see the problem.</p>      <p>I got the encoded object back from the service as a get and tryed to put it back so the object is encoded correctly</p>      <p>Hope someone can help find the solution cos I think it is come detail I missed.</p>      <p><strong>Fiddler call:</strong></p>      <pre><code>http://Server/WcfSyncDBService/SyncDBService.svc/AddTodoItems/PEFycmF5T2ZJdGVtcyB4bWxucz0iaHR0cDovL3NjaGVtYXMuZGF0YWNvbnRyYWN0Lm9yZy8yMDA0LzA3L1djZlN5bmNEQlNlcnZpY2UiIHhtbG5zOmk9Imh0dHA6Ly93d3cudzMub3JnLzIwMDEvWE1MU2NoZW1hLWluc3RhbmNlIj48SXRlbXM+PENhdGVnb3J5PlJlbWluZGVyPC9DYXRlZ29yeT48RGVzY3JpcHRpb24+UmVtaW5kZXI8L0Rlc2NyaXB0aW9uPjxJZD41PC9JZD48U3VtbWFyeT5UZXN0IDU8L1N1bW1hcnk+PC9JdGVtcz48L0FycmF5T2ZJdGVtcz4=      </code></pre>      <p><strong>here is my code:</strong></p>      <p>Interface:</p>      <pre><code>[OperationContract(Name = \"AddTodoItems\")]      [WebInvoke(Method = \"POST\", RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare,      ResponseFormat = WebMessageFormat.Json, UriTemplate = \"AddTodoItems/{content}\")]      string AddTodoItems(string content);      </code></pre>      <p>Implementation:</p>      <pre><code>public string AddTodoItems(string content)      {      var ret = String.Empty;      try      {      var data = DecodeFrom64(content);      ret = \"Encoded:&lt;\" + content + \"&gt;\";      ret = ret + \"content:&lt;\" + data+\"&gt;\";      var ms = new MemoryStream(Encoding.UTF8.GetBytes(data));      var serializer = new DataContractSerializer(typeof(TodoItem[]));      var todoArray = (TodoItem[])serializer.ReadObject(ms);      var todoList = todoArray.ToList();      if (todoList.Count &gt; 0)      {      var context = new SyncDBEntities();      foreach (var item in todoList)      {      var dbItem = (from i in context.todo where i.C_id == item.Id select i).First();      if (dbItem == null ) // insert      {      var itemAdd = new SyncDBmodel.todo();      itemAdd.C_id = item.Id;      itemAdd.category = item.Category;      itemAdd.summary = item.Summary;      itemAdd.description = item.Description;      context.AddTotodo(itemAdd);      context.SaveChanges();      }      else // update      {      dbItem.C_id = item.Id;      dbItem.category = item.Category;      dbItem.summary = item.Summary;      dbItem.description = item.Description;      context.SaveChanges();      }      }      }      }      catch (Exception e)      {      ret = ret+\"Error:&lt;\"+e.Message+\"&gt;\";      ret = ret + \"InnerException:&lt;\" + e.InnerException + \"&gt;\";      }      finally      {      if (String.IsNullOrEmpty(ret))      ret = \"OK\";      }      return ret;      }      public bool DelTodoItems(string content)      {      bool ret = true;      try      {      var ms = new MemoryStream(Encoding.UTF8.GetBytes(content));      var serializer = new DataContractSerializer(typeof(TodoItem[]));      var todoArray = (TodoItem[])serializer.ReadObject(ms);      var todoList = todoArray.ToList();      if (todoList.Count &gt; 0)      {      var context = new SyncDBEntities();      foreach (var item in todoList)      {      var dbItem = (from i in context.todo where i.C_id == item.Id select i).First();      if (dbItem != null) // delete      {      context.DeleteObject(dbItem);      context.SaveChanges();      }      }      }      }      catch (Exception)      {      ret = false;      }      return ret;      }      </code></pre>      <p>Web.config:</p>      <pre><code>&lt;?xml version=\"1.0\"?&gt;      &lt;configuration&gt;      &lt;system.web&gt;      &lt;compilation debug=\"true\" targetFramework=\"4.0\" /&gt;      &lt;httpRuntime maxRequestLength=\"524288\" /&gt;      &lt;/system.web&gt;      &lt;system.serviceModel&gt;      &lt;bindings&gt;      &lt;webHttpBinding&gt;      &lt;binding name=\"StreamedRequestWebBinding\"      bypassProxyOnLocal=\"true\"      useDefaultWebProxy=\"false\"      hostNameComparisonMode=\"WeakWildcard\"      sendTimeout=\"10:15:00\"      openTimeout=\"10:15:00\"      receiveTimeout=\"10:15:00\"      maxReceivedMessageSize=\"2147483647\"      maxBufferSize=\"2147483647\"      maxBufferPoolSize=\"2147483647\"      transferMode=\"StreamedRequest\"&gt;      &lt;readerQuotas maxDepth=\"2147483647\"      maxArrayLength=\"2147483647\"      maxStringContentLength=\"2147483647\"      maxBytesPerRead=\"2147483647\"/&gt;      &lt;security mode=\"None\"/&gt;      &lt;/binding&gt;      &lt;/webHttpBinding&gt;      &lt;/bindings&gt;      &lt;services&gt;      &lt;service name=\"WcfSyncDBService.SyncDBService\" behaviorConfiguration=\"ServiceBehaviour\"&gt;      &lt;endpoint      address =\"\"      binding=\"webHttpBinding\"      bindingConfiguration=\"StreamedRequestWebBinding\"      contract=\"WcfSyncDBService.ISyncDBService\"      behaviorConfiguration=\"web\"&gt;      &lt;/endpoint&gt;      &lt;/service&gt;      &lt;/services&gt;      &lt;behaviors&gt;      &lt;serviceBehaviors&gt;      &lt;behavior name=\"ServiceBehaviour\"&gt;      &lt;serviceMetadata httpGetEnabled=\"true\"/&gt;      &lt;serviceDebug includeExceptionDetailInFaults=\"false\"/&gt;      &lt;/behavior&gt;      &lt;/serviceBehaviors&gt;      &lt;endpointBehaviors&gt;      &lt;behavior name=\"web\"&gt;      &lt;webHttp/&gt;      &lt;/behavior&gt;      &lt;/endpointBehaviors&gt;      &lt;/behaviors&gt;      &lt;serviceHostingEnvironment multipleSiteBindingsEnabled=\"true\" /&gt;      &lt;/system.serviceModel&gt;      &lt;system.webServer&gt;      &lt;modules runAllManagedModulesForAllRequests=\"true\"/&gt;      &lt;httpCompression directory=\"%SystemDrive%\\inetpub\\temp\\IIS Temporary Compressed Files\"&gt;      &lt;scheme name=\"gzip\" dll=\"%Windir%\\system32\\inetsrv\\gzip.dll\"/&gt;      &lt;dynamicTypes&gt;      &lt;add mimeType=\"text/*\" enabled=\"true\"/&gt;      &lt;add mimeType=\"application/xml\" enabled=\"true\" /&gt;      &lt;add mimeType=\"application/json\" enabled=\"true\" /&gt;      &lt;add mimeType=\"message/*\" enabled=\"true\"/&gt;      &lt;add mimeType=\"application/javascript\" enabled=\"true\"/&gt;      &lt;add mimeType=\"*/*\" enabled=\"false\"/&gt;      &lt;/dynamicTypes&gt;      &lt;staticTypes&gt;      &lt;add mimeType=\"text/*\" enabled=\"true\"/&gt;      &lt;add mimeType=\"message/*\" enabled=\"true\"/&gt;      &lt;add mimeType=\"application/javascript\" enabled=\"true\"/&gt;      &lt;add mimeType=\"*/*\" enabled=\"false\"/&gt;      &lt;/staticTypes&gt;      &lt;/httpCompression&gt;      &lt;/system.webServer&gt;      &lt;connectionStrings&gt;      &lt;add name=\"SyncDBEntities\" connectionString=\"metadata=res://*/SyncDBmodel.csdl|res://*/SyncDBmodel.ssdl|res://*/SyncDBmodel.msl;provider=System.Data.SqlClient;provider connection string=&amp;quot;data source=.;initial catalog=SyncDB;Persist Security Info=True;User ID=sa;Password=isql;multipleactiveresultsets=True;App=EntityFramework&amp;quot;\" providerName=\"System.Data.EntityClient\" /&gt;      &lt;/connectionStrings&gt;      &lt;/configuration&gt;      </code></pre>",
            tags: "<c#><json><wcf><rest><post>"
            );

            posts.Add(
            id: 18651878,
            title: "How can I send a DataContract object as parameter for a WCF RESTFUL webservice?",
            parentId: null,
            acceptedAnswerId: 18656905,
            creationDate: DateTime.Parse("2013-09-06T07:06:41"),
            body: "<p>I am developing a WCF resful service which will be basically consumed from some mobile applications.      Over the POST I am trying to send a DataContract object [actually I have to send a List of object] and another single id as string. My question is if it is possibly to define my function to accept DataContract object and the single string ?</p>      <p>Following is my code :      Interface declaration:</p>      <pre><code>[ServiceContract]      public interface IService1      {      [OperationContract]      [WebInvoke(Method = \"POST\", ResponseFormat = WebMessageFormat.Json, UriTemplate = \"GetDataUsingDataContract/{id}\")]      CompositeType GetDataUsingDataContract(string id, CompositeType composite );      }      [DataContract]      public class CompositeType      {      bool boolValue = true;      string stringValue = \"Hello \";      [DataMember]      public bool BoolValue      {      get { return boolValue; }      set { boolValue = value; }      }      [DataMember]      public string StringValue      {      get { return stringValue; }      set { stringValue = value; }      }      }      </code></pre>      <p>Actual definition of the function:</p>      <pre><code>public CompositeType GetDataUsingDataContract(string id, CompositeType composite )      {      if (composite == null)      {      throw new ArgumentNullException(\"composite\");      }      if (composite .BoolValue)      {      composite .StringValue += \"- Suffix and the id is\"+id;      }      return report;      }      </code></pre>      <p>and the json object I am trying to send from Fiddler is </p>      <pre><code>{\"BoolValue\":true,\"StringValue\":\"sdfsdfsf\"}      </code></pre>      <p><img src=\"http://i.stack.imgur.com/xrtzt.jpg\" alt=\"Fiddler snap when sending the request\">      <img src=\"http://i.stack.imgur.com/slfkc.jpg\" alt=\"Fiddler snap of the output\">      Above are the snaps from the fiddler I am testing the service from.      After couple of googling I have got the following link where the client actually uses webservice reference to get the DataContract type and serializes to json before sending as request body. But why then my test from Fiddler doesn't succeed ?!      <a href=\"http://dotnetmentors.com/wcf/wcf-rest-service-to-get-or-post-json-data-and-retrieve-json-data-with-datacontract.aspx\" rel=\"nofollow\">http://dotnetmentors.com/wcf/wcf-rest-service-to-get-or-post-json-data-and-retrieve-json-data-with-datacontract.aspx</a></p>      <p>Can anybody please suggest anything ?</p>      <p>The web.config is as bellow:</p>      <pre><code>&lt;?xml version=\"1.0\"?&gt;      &lt;configuration&gt;      &lt;system.web&gt;      &lt;compilation debug=\"true\" targetFramework=\"4.0\" /&gt;      &lt;/system.web&gt;      &lt;system.serviceModel&gt;      &lt;services&gt;      &lt;service name=\"JSONWebService.Service1\" behaviorConfiguration=\"JSONWebService.Service1Behavior\"&gt;      &lt;endpoint address=\"../Service1.svc\"      binding=\"webHttpBinding\"      contract=\"JSONWebService.IService1\"      behaviorConfiguration=\"webBehaviour\" /&gt;      &lt;/service&gt;      &lt;/services&gt;      &lt;behaviors&gt;      &lt;serviceBehaviors&gt;      &lt;behavior name=\"JSONWebService.Service1Behavior\"&gt;      &lt;!-- To avoid disclosing metadata information, set the value below to false and remove the metadata endpoint above before deployment --&gt;      &lt;serviceMetadata httpGetEnabled=\"true\"/&gt;      &lt;!-- To receive exception details in faults for debugging purposes, set the value below to true.  Set to false before deployment to avoid disclosing exception information --&gt;      &lt;serviceDebug includeExceptionDetailInFaults=\"true\"/&gt;      &lt;/behavior&gt;      &lt;/serviceBehaviors&gt;      &lt;endpointBehaviors&gt;      &lt;behavior name=\"webBehaviour\"&gt;      &lt;webHttp/&gt;      &lt;/behavior&gt;      &lt;/endpointBehaviors&gt;      &lt;/behaviors&gt;      &lt;serviceHostingEnvironment multipleSiteBindingsEnabled=\"true\" /&gt;      &lt;/system.serviceModel&gt;      &lt;system.webServer&gt;      &lt;modules runAllManagedModulesForAllRequests=\"true\"/&gt;      &lt;/system.webServer&gt;      &lt;/configuration&gt;      </code></pre>",
            tags: "<json><wcf><rest><wcf-rest>"
            );

            posts.Add(
            id: 18650593,
            title: "How to call onSubmit from REST api",
            parentId: null,
            acceptedAnswerId: null,
            creationDate: DateTime.Parse("2013-09-06T05:33:53"),
            body: "<p>I am trying to write a rest api to update the details via <code>ModelAndView onSubmit(HttpServletRequest request,HttpServletResponse response, Object command, BindException errors)</code> may I know how to send request and responce parameter to it along with object from rest api</p>",
            tags: "<spring><rest>"
            );

            posts.Add(
            id: 18646747,
            title: "How to produce JSON output with Jersey 1.17.1 using JAXB",
            parentId: null,
            acceptedAnswerId: 18662286,
            creationDate: DateTime.Parse("2013-09-05T22:08:34"),
            body: "<p>There is a correct answer to this question already on this site. The problem is that the <a href=\"http://stackoverflow.com/questions/6027097/how-to-produce-json-output-with-jersey-1-6-using-jaxb\">question is for Jersey 1.6</a> and <a href=\"http://stackoverflow.com/a/15662700/61624\">the correct answer for Jersey 1.17.1</a> is buried at the bottom.  I figured I'd create a correct question for this answer so that it'd be easier to find help for people struggling with this (like I was).</p>",
            tags: "<rest><jaxb><jersey><jackson><jersey-1.0>"
            );

            posts.Add(
            id: 18645547,
            title: "Teamcity REST API get latest successful build on a branch",
            parentId: null,
            acceptedAnswerId: null,
            creationDate: DateTime.Parse("2013-09-05T20:40:08"),
            body: "<p>I'm using git flow with teamcity as my CI server. I'd like to pull artifacts from the latest successful build on a particular branch.</p>      <p>I can use this url to get the latest build on a branch:      http://$teamcity$/httpAuth/app/rest/buildTypes/name:$BuildTypeName$/builds/branch:name:$branchName$</p>      <p>but it fails if the branch name contains '/' (e.g., git flow names branches 'feature/%' and 'release/%').</p>      <p>I've tried url encoding the '/'. For example, if $branchName$> == 'release/branchName' I use /builds/branch:name:release%2F$branchName$).</p>      <ul>      <li>works - /builds/branch:name:develop</li>      <li>fails - /builds/branch:name:release%2F$branchName$.</li>      </ul>      <p>I don't get an api error, but the api result is empty.</p>",
            tags: "<git><rest><teamcity>"
            );

            posts.Add(
            id: 18645434,
            title: "UnSupported Media Type when Calling REST Web Service",
            parentId: null,
            acceptedAnswerId: 18645849,
            creationDate: DateTime.Parse("2013-09-05T20:33:42"),
            body: "<p>I am calling a REST web service which has given me this documentation</p>      <pre><code>HTTP Method: POST      Path: /commit/{path}/add-node      Response Status 200, 302, 403, 404, 409, 503      Form Parameters      - name : attribute name      - message : commit message      </code></pre>      <p>Based on this documentation. I have written following C# code.</p>      <pre><code>  string restUrl = webServiceurl + \"/commit/\" + path + \"/add-node\";      restUrl = restUrl + \"?name=\" + nodeName + \"&amp;message=\" + commitMessage;      HttpWebRequest request = (HttpWebRequest)WebRequest.Create(restUrl);      request.Method = \"POST\";      request.ContentType = @\"application/json\";      using (WebResponse response = request.GetResponse()) {      using (StreamReader reader = new StreamReader(response.GetResponseStream())) {      output = reader.ReadToEnd();      }      }      </code></pre>      <p>I also tried</p>      <pre><code>  string restUrl = webServiceurl + \"/commit/\" + path + \"/add-node\";      HttpWebRequest request = (HttpWebRequest)WebRequest.Create(restUrl);      request.Method = \"POST\";      request.ContentType = @\"application/json\";      var param = new { name = nodeName, message = commitMessage };      Stream reqStream = null;      string output = null;      try {      byte[] buffer = Encoding.GetEncoding(\"UTF-8\").GetBytes(      JsonConvert.SerializeObject(param)      );      request.ContentLength = buffer.Length;      reqStream = request.GetRequestStream();      reqStream.Write(buffer, 0, buffer.Length);      using (WebResponse response = request.GetResponse()) {      using (StreamReader reader = new StreamReader(response.GetResponseStream())) {      output = reader.ReadToEnd();      }      }      } catch (Exception ex) {      .....      }      </code></pre>      <p>Unfortunately in both cases, I get 415 Unsupported Media Type in both cases. What is wrong with my code? </p>      <p>The web Services is a REST based web service written in Java.</p>",
            tags: "<c#><rest>"
            );

            posts.Add(
            id: 18644029,
            title: "Python/Django REST API Architecture",
            parentId: null,
            acceptedAnswerId: 18644525,
            creationDate: DateTime.Parse("2013-09-05T19:04:35"),
            body: "<p>I'm trying to build a niche social network like Instagram as a Python/Django application.</p>      <p>So the things I need, regarding architecture, are (I guess):</p>      <ol>      <li>REST API (e.g. api.mystagram.com).</li>      <li>Public website (www.mystagram.com or mystagram.com).</li>      <li>URL shortener (e.g. mystagr.am).</li>      <li>Android app</li>      <li>iPhone app</li>      <li>Windows Phone app</li>      <li>...</li>      </ol>      <p>Before this I only built simple to some less-simple websites, but never extremely complex with own custom APIs or so. I have never build my own REST API before (I have used other REST APIs though) or even built an Android/iPhone app and distributed it in the Play Store/App Store (I have made some typical hello world examples though).</p>      <p>So, the most important thing to me seems to create a kick-ass REST API first and proceed from there. I am blocked however by a few questions.</p>      <ol>      <li>How should I organize the projects for the public website and REST API? Should these be separate Django projects or should I create only one Django project and add both the public website and REST API as an internal Django module?</li>      <li>Should the public website also make use of the REST API? Or is it better to just use the plain Django models for this?</li>      </ol>      <p>Thanks in advance for any help! If somebody knows some great presentations or so on this topic (architecture), always welcome!</p>      <p>Kind regards,      Kristof</p>",
            tags: "<python><django><api><rest><architecture>"
            );

            posts.Add(
            id: 18642446,
            title: "Amazon S3: Cache-Control and Expiry Date difference and setting trough REST API",
            parentId: null,
            acceptedAnswerId: null,
            creationDate: DateTime.Parse("2013-09-05T17:27:49"),
            body: "<p>I want to enhance my sites loading speed, so I use <a href=\"http://gtmetrix.com/\" rel=\"nofollow\">http://gtmetrix.com/</a>, to check what I could improve. One of the lowest rating I get for \"Leverage browser caching\". I found, that my files (mainly images), have problem \"expiration not specified\".</p>      <p>Okay, problem is clear, I thought. I start to googling and I found that amazon S3 prefer Cache-Control meta data over Expiry date (I lost this link, now I think maybe I misunderstood something). Anyway, I start looking for how to add cache-control meta to S3 object. I found      this page: <a href=\"http://www.bucketexplorer.com/documentation/amazon-s3--how-to-set-cache-control-header-for-s3-object.html\" rel=\"nofollow\">http://www.bucketexplorer.com/documentation/amazon-s3--how-to-set-cache-control-header-for-s3-object.html</a></p>      <p>I learned, that I must add string to my PUT query.</p>      <p><code>x-amz-meta-Cache-Control : max-age= &lt;value in seconds&gt;</code> //(there is no need space between equal sign and digits(I made a mistake here)).</p>      <p>I use construction: <code>Cache-control:max-age=1296000</code> and it work okay. </p>      <p>After that I read      <a href=\"https://developers.google.com/speed/docs/best-practices/caching\" rel=\"nofollow\">https://developers.google.com/speed/docs/best-practices/caching</a>      This article told me: 1) \"Set Expires to a minimum of one month, and preferably up to one year, in the future.\" </p>      <p>2) \"We prefer Expires over Cache-Control: max-age because it is is more widely supported.\"(in Recommendations topic).  </p>      <p>So, I start to look way to set Expiry date to S3 object.      I found this:      <a href=\"http://www.bucketexplorer.com/documentation/amazon-s3--set-object-expiration-on-amazon-s3-objects-put-get-delete-bucket-lifecycle.html\" rel=\"nofollow\">http://www.bucketexplorer.com/documentation/amazon-s3--set-object-expiration-on-amazon-s3-objects-put-get-delete-bucket-lifecycle.html</a></p>      <p>And what I found: \"Using Amazon S3 Object Lifecycle Management , you can define the Object Expiration on Amazon S3 Objects . Once the Lifecycle defined for the S3 Object expires, Amazon S3 will delete such Objects. So, when you want to keep your data on S3 for a limited time only and you want it to be deleted automatically by Amazon S3, you can set Object Expiration.\"</p>      <p>I don't want to delete my files from S3. I just want add cache meta for maximum cache time or/and file expiry time.</p>      <p>I completely confused with this. Can somebody explain what I must use: object expiration or cache-control?</p>",
            tags: "<rest><caching><amazon-s3><metadata><cache-control>"
            );

            posts.Add(
            id: 18638320,
            title: "Downloading Documents from SugarCRM without being embedded in JSONObject",
            parentId: null,
            acceptedAnswerId: null,
            creationDate: DateTime.Parse("2013-09-05T13:56:04"),
            body: "<p>I want to download the document from SugarCRM. I am using JSONObject and accessing through REST API on JAVA. I used the *get_document_revision* method from REST. As a result, I get the JOSN response and the binary contents are part of the JSONObject. If I want to retrieve the binary file only, I have to load the whole content I got, as response, in the memory, to separate the file and other JSON fields. The file could be arbitrary large, so, I cannot afford to load the whole file in the memory in general.      I want to ask if there is a way, to download the file alone, without being bundled in JSONObject. </p>",
            tags: "<java><json><rest><file-download><sugarcrm>"
            );

            posts.Add(
            id: 18636687,
            title: "Preventing modification of specific properties on entity",
            parentId: null,
            acceptedAnswerId: 18637774,
            creationDate: DateTime.Parse("2013-09-05T12:43:21"),
            body: "<p>I am exposing an entity from my database thru an OData feed based on WCF DataServices in .Net 4.0 . Up until now everything has been completely open, but I am now in the process of limiting the operations possible on the entities.</p>      <p>I have an Order-object with these properties (amongst others):</p>      <p>ID</p>      <p>Name</p>      <p>Amount</p>      <p>CustomerID</p>      <p>I would like to be able to expose all values to the consumer of the service and allow them to update them. However - I don't want them to be able to update the CustomerID-property of the entity.</p>      <p>How can I accomplish this? I have looked into QueryInterceptors, but I have not yet found the right way to either block the update call or modify the request.</p>      <p>Can you guys help me here?</p>      <p>Thanks,</p>      <p>/Jesper</p>      <p>Copenhagen, Denmark</p>",
            tags: "<wcf-data-services><odata>"
            );

            posts.Add(
            id: 18633129,
            title: "SharePoint REST and setting Metadata columns",
            parentId: null,
            acceptedAnswerId: null,
            creationDate: DateTime.Parse("2013-09-05T09:52:50"),
            body: "<p>I have a jquery script that uploads a file to document library.</p>      <p>I'm trying to set a choice column in the same call, but can't seem to get it to work. The upload is working, but Col1 is a choice column and 'Personal Statement' is a valid choice, but it does not take so I end up with the default value.</p>      <pre><code>reqExecutor.executeAsync({      url: url,      method: \"POST\",      headers: {      \"Accept\": \"application/json; odata=verbose\",      \"X-RequestDigest\": digest      },      data: JSON.stringify({ '__metadata': { 'type': 'SP.List', 'co1': 'Personal Statement' }, 'Title': 'New title'  }),      contentType: \"application/json;odata=verbose\",      binaryStringRequestBody: true,      body: fileData,      success:successHandler,      error: errorHandler      </code></pre>",
            tags: "<jquery><json><rest><sharepoint2013>"
            );

            posts.Add(
            id: 18630477,
            title: "About: get REST - returning a NotFound exception C#",
            parentId: null,
            acceptedAnswerId: null,
            creationDate: DateTime.Parse("2013-09-05T07:40:41"),
            body: "<p>Attempting to retrieve quota information from Google Drive via REST in C#.  Here is a code snippet:</p>      <pre><code>public async Task&lt;string&gt; HasQuota(string accessToken) {      var url = string.Concat(new string[] {      \" https://www.googleapis.com/drive/v2/about\",      \"?access_token=\" + accessToken,      \"&amp;fields=quotaBytesTotal%2CquotaBytesUsed\"      });      // Create the request.      var request = WebRequest.Create(url) as HttpWebRequest;      request.Method = \"GET\";      // Create the response.      var response = await Task&lt;WebResponse&gt;.Factory.FromAsync(      request.BeginGetResponse,      request.EndGetResponse,      request      );      if (((HttpWebResponse)response).StatusDescription == \"OK\") {      using (Stream stream = response.GetResponseStream()) {      StreamReader reader = new StreamReader(stream, Encoding.UTF8);      return reader.ReadToEnd();      }      }      return null;      }      </code></pre>      <p>When I obtained the access_token I scoped the authentication request with <a href=\"https://www.googleapis.com/auth/drive.file\" rel=\"nofollow\">https://www.googleapis.com/auth/drive.file</a> and my test user granted permissions.  So as far as I aware I have the code requirements to execute this call.</p>      <p>However I get an NotFound exception when I attempt to execute this method.  The documentation says I can pass the access_token in the query string or add the Authorization header to the request i.e</p>      <pre><code>request.Headers[\"Authorization\"] = string.Format(\"Bearer {0}\", accessToken);      </code></pre>      <p>Any ideas why I might be getting this exception?</p>      <p>Thanks</p>",
            tags: "<c#><rest><google-api>"
            );

            posts.Add(
            id: 18630456,
            title: "Should API REST BDD Cucumbers Gherkins include specific details about an API or data",
            parentId: null,
            acceptedAnswerId: 0,
            creationDate: DateTime.Parse("2013-09-05T07:39:17"),
            body: "<p>I'd like to pose this question since it seems to be a point of some debate and I'd like to know the communities thoughts on it.</p>      <p>To give you a bit of background to the way the team I work in operates and give this question some context, we're writing cucumbers for a RESTful API at a session called 'Three Amigos'. Three Amigos basically means there will be a Tech Lead, Developer (one or more) and BA (one or more) involved in fleshing out the acceptance criteria for a story. As part of this session the BA usually drives writing the gherkins for the cucumbers.</p>      <p>Here's an example to kick it off. If I have a RESTful API for getting back information about a car I may have a scenario that says:-</p>      <pre><code>Scenario: Engine size should appear in the car      Given a car exists      When I request the car      Then the car should have a \"1700cc\" engine capacity      </code></pre>      <p>Or you could write it like</p>      <pre><code>Scenario: Engine size should appear in the car      Given a \"Mazda/ModelABC\" car exists with an engine capacity      When I GET \"Mazda/ModelABC\"      Then the response should contain \"1700cc\" engine capacity      </code></pre>      <p>Now the first one in my eyes is easier all round to read but will not promote code re-use (is this a big deal?). The second promotes code re-use and is written from the perspective of a stakeholder i.e. a developer but a Business Analyst (BA) would not write it like this so it would make a Three Amigos session fairly pointless.</p>      <p>Given the two approaches which is the more highly recommended choice? I have opted for the first approach in my case but I'm interested to know what the arguments for either method are or if there are some decent articles people can back up the suggestions with that would suggest which approach should really be used.</p>      <p>Thanks.</p>",
            tags: "<rest><cucumber><bdd><acceptance-testing><gherkin>"
            );

            posts.Add(
            id: 18627989,
            title: "How to Best Test if an Auth Token is Valid - Django REST Framework",
            parentId: null,
            acceptedAnswerId: null,
            creationDate: DateTime.Parse("2013-09-05T04:51:51"),
            body: "<p>I am writing a program to interact with my Django REST Framework API. The program needs to check if a token is valid (and nothing else). What is the best way to do this? Can I add a new view to the API to accomplish this? I have a feeling there is an easy trick that I am missing.</p>      <p>Thanks</p>",
            tags: "<python><django><rest><restful-authentication>"
            );

            posts.Add(
            id: 18627862,
            title: "Does REST breaks data encapsulation?",
            parentId: null,
            acceptedAnswerId: 0,
            creationDate: DateTime.Parse("2013-09-05T04:38:25"),
            body: "<p>I am trying to understand the scope and limits of RESTful APIs. My specific question is this: How can I handle with REST an API that exposes operations rather than resources? Should I give up to the temptation of exposing operations and rethink the API to expose data (resources). Coming from OOP that feels like a blatant violation of object encapsulation. </p>      <p>Imagine that you need to expose a REST API that does a money transfer: transfers an amount from one account into another account. If I understand REST, the two accounts should be exposed as resources and two different UPDATE operations would have to be invoked on these two resources. To me, this feels like a clear violation of the data encapsulation. My tendency is to create an API that models the operation “transfer money” rather than the resource “account”. Can I create a REST API that does the “data transfer”? Is that no longer REST (since it does not appear to be resource centric). </p>      <p>Any comment on this scenarios where RPC calls look more appropriate than REST?</p>      <p>Thanks</p>",
            tags: "<api><oop><rest>"
            );

            posts.Add(
            id: 18619891,
            title: "Apache CXF Rest Client Clarity",
            parentId: null,
            acceptedAnswerId: null,
            creationDate: DateTime.Parse("2013-09-04T17:08:22"),
            body: "<p>My requirement is to use Apache CXF Rest Client API      vendor has provided an url <a href=\"http://test.com\" rel=\"nofollow\">http://test.com</a></p>      <pre><code>Method: get      Url: /getDetails      Header set to application/json      and parameter z=12345      </code></pre>      <p>And Response as JSON:</p>      <pre><code>{      \"hasMore\":\"true\",      \"results\":[      {      \"name\":\"ATM:      16th      &amp;      Treemont\",      \"phone\":\"(303)      249---9117\",      \"streetAddress\":\"303      16th      St.      Suite      100\"},{      \"name\":\"ATM2:      17th      &amp;      Fremont\",      \"phone\":\"(555)      999-98886\",      \"streetAddress\":\"304      17th      St.      Suite      200\"}]      }      </code></pre>      <p>When I read the documentation for the Client API I see      link:      <a href=\"http://cxf.apache.org/docs/jax-rs-client-api.html#JAX-RSClientAPI-CXFWebClientAPI\" rel=\"nofollow\">http://cxf.apache.org/docs/jax-rs-client-api.html#JAX-RSClientAPI-CXFWebClientAPI</a>      WebClient approach:      if i go by this example, it describes about Book() how do I describe Book object for my requirement?</p>      <pre><code>WebClient client = WebClient.create(\"http://books\");      client.path(\"bookstore/books\");      client.type(\"text/xml\").accept(\"text/xml\")      Response r = client.post(new Book());      Book b = r.readEntity(Book.class);      </code></pre>      <p>Also I see usage of Proxy: It talks about BookStore.class ..wont this be the server object? if so I will not be able to create or have BookStore class or object at my end.</p>      <pre><code>BookStore store = JAXRSClientFactory.create(\"http://bookstore.com\", BookStore.class);      // (1) remote GET call to http://bookstore.com/bookstore      Books books = store.getAllBooks();      // (2) no remote call      BookResource subresource = store.getBookSubresource(1);      // {3} remote GET call to http://bookstore.com/bookstore/1      Book b = subresource.getBook();      </code></pre>      <p>Should I create a Object similar to Book() for my response? I actually have to read each value from the JSON response (jettison).      Which approach should I follow for my reqruiement and how should I proceed. I am confused please advice.</p>      <p>My requirement is strict to use Apache CXF Rest API.</p>",
            tags: "<apache><rest><cxf><jettison>"
            );

            posts.Add(
            id: 18614740,
            title: "How to implement authentication in REST API?",
            parentId: null,
            acceptedAnswerId: null,
            creationDate: DateTime.Parse("2013-09-04T13:07:37"),
            body: "<p>We are developing a REST API and want to implement a authentication scheme. I will describe what we are doing.</p>      <ol>      <li>In the first request by user the credentials (username / password) will be passed.</li>      <li>The credentials will be authenticated.</li>      <li>An authentication token will be generated and passed back to the client. The authentication token will have an expiry time.</li>      <li>For any subsequent requests by the client the authentication token will be used to access the resource.</li>      <li>If authentication is expired then error message will be provided saying pass on the credentials to re-generate the authentication token.</li>      </ol>      <p>I have some doubts on this.      What if two requests are made with same credentials from different sources like 2 different PCs or even mobile device? Should we generate two different authentication tokens for the same?      Or may be we should expect username / password every time a resource is requested?</p>      <p>As I have read <a href=\"http://stackoverflow.com/questions/6068113/do-sessions-really-violate-restfulness\">here</a>, it does recommend having same token for same user across devices or nodes in a distributed environment.</p>      <p>So can some one help me on this?</p>      <p>I can not really approach oAuth or such frameworks since we have very less time.</p>",
            tags: "<rest>"
            );

            posts.Add(
            id: 18612814,
            title: "How get all infomation about deployed REST services in Glassfish admin console?",
            parentId: null,
            acceptedAnswerId: null,
            creationDate: DateTime.Parse("2013-09-04T11:38:26"),
            body: "<p>If I annotate some class with <code>@WebService</code> compile and deploy it into glassfish. I can go to admin console find my class click to \"View Endpoint\" link and get all necessary information about service. How I can do that with Jersey? I have class with <code>@Path</code> annotation, and methods with <code>@Produces</code>, <code>@Post</code>, <code>@Get</code> etc. annotations. Where I can find information about all these methods in glassfish? Where find a link with generated wadl file?</p>",
            tags: "<java><rest><glassfish><jersey><jax-ws>"
            );

            posts.Add(
            id: 18611695,
            title: "How to search for projects under ScopeLabel using VersionOne REST Api?",
            parentId: null,
            acceptedAnswerId: null,
            creationDate: DateTime.Parse("2013-09-04T10:45:07"),
            body: "<p>How to search for projects under ScopeLabel using VersionOne REST Api? </p>      <p>I came to know that the Programs listed in the VersionOne site could be found using ScopeLabel in the REST API as follows - <a href=\"http://v1.com/v1/rest-1.v1/data/scopelabel/10\" rel=\"nofollow\">http://v1.com/v1/rest-1.v1/data/scopelabel/10</a>.</p>      <p>But, there are projects listed under the Program section. So, I don't know how to proceed further using ScopeLabel to get to know the projects under the program. Need help in this regard. I'm looking forward to an URL like solution as shown above, as it would help me moving ahead.</p>      <p>Thanks in advance !!!</p>",
            tags: "<c#><rest><versionone>"
            );

            posts.Add(
            id: 18667203,
            title: "Connect to .net MVC FROM Salesforce with REST API",
            parentId: null,
            acceptedAnswerId: null,
            creationDate: DateTime.Parse("2013-09-06T21:57:52"),
            body: "<p>Hi I am wondering if it is possible to retrieve data from a MVC .net application into Salesforce using REST API in APEX. </p>      <p>Thanks</p>",
            tags: "<asp.net-mvc><rest><salesforce>"
            );

            posts.Add(
            id: 18667153,
            title: "Returning a long array of JSON objects in a REST API",
            parentId: null,
            acceptedAnswerId: null,
            creationDate: DateTime.Parse("2013-09-06T21:54:16"),
            body: "<p>Suppose I have a large (but finite) array of JSON objects I want to return from a REST API. The standard way to do it would be to return the array in its entirety, which means any client must wait until the entire array is finished downloading before it can be interpreted.</p>      <p>How might one return said array in a way that allows the objects to be processed one at a time?</p>      <p>Were I using <code>bottle</code> and <code>urllib2</code> libraries from Python, I'd imagine an interface like so,</p>      <p><code>server.py</code></p>      <pre><code>@bottle.get(\"/long/array\")  # reachable from http://localhost/long/array      @streaming_json             # indicates that this function returns a generator of JSON-serializable objects      def long_array():      for obj in really_long_array:      yield obj      </code></pre>      <p><code>client.py</code></p>      <pre><code>for line in urllib2.urlopen(\"http://localhost/long/array\"):      print json.loads(obj)      </code></pre>      <p>Does such an interface exist? How might I go about implementing one?</p>",
            tags: "<python><json><rest><streaming>"
            );

            posts.Add(
            id: 18665617,
            title: "Can jersey clients POST a JAXB object to the server using JSON?",
            parentId: null,
            acceptedAnswerId: 18665641,
            creationDate: DateTime.Parse("2013-09-06T19:54:37"),
            body: "<p>I'm finding a lot of examples of how to set up a jersey server so that it can produce and consume JAXB bound objects but I'm having trouble finding examples of how to get the client to post the same JAXB bound object.  <a href=\"http://www.javacodegeeks.com/2012/10/rest-using-jersey-complete-tutorial-with-jaxb-exception-handling-and-client-program.html\" rel=\"nofollow\">This example shows how to do it with XML</a>.  I'm looking for one that shows how to do it with JSON.  </p>      <p>I'm not even sure if this is possible to do.  The javadoc on the <a href=\"http://jersey.java.net/nonav/apidocs/1.4/jersey/com/sun/jersey/api/client/WebResource.html#post%28%29\" rel=\"nofollow\">post</a> method(s) are ambiguous.  </p>      <p>My post looks like this:</p>      <pre><code>    Client client = Client.create();      WebResource resource = client.resource(uri);      ClientResponse response = resource.type(MediaType.APPLICATION_JSON).post(ClientResponse.class, instanceWithXmlRootElementAnnotation);      </code></pre>      <p>When I try this, my server gets the request, but the field for the <code>@FormParam</code> is always sent over as null.  Here's the signature of my server side method:</p>      <pre><code>@POST      @Path(\"apath\")      @Consumes(MediaType.APPLICATION_JSON)      public String postAPath(@FormParam(\"InstanceWithXmlRootElementAnnotation\") InstanceWithXmlRootElementAnnotation instanceWithXmlRootElementAnnotation) {      //instanceWithXmlRootElementAnnotation is always null      </code></pre>      <p>Something else I'm wondering is if I should be using <code>instanceWithXmlRootElementAnnotation</code>.  If this were a traditional webservice, I would use JAXB to <em>generate</em> an object for the client to use and send over the generated class.  But from what I gather from the example I linked to, the guy is sending over the source, not the generated class.  </p>",
            tags: "<rest><jersey><jersey-client><jersey-1.0>"
            );

            posts.Add(
            id: 18663366,
            title: "Sign envelope through REST API?",
            parentId: null,
            acceptedAnswerId: 18663668,
            creationDate: DateTime.Parse("2013-09-06T17:29:43"),
            body: "<p>Is it possible to actually sign an envelope through the REST API? Not just get an embedded signer Url token, but actually sign the envelope?</p>",
            tags: "<rest><docusignapi>"
            );

            posts.Add(
            id: 18661328,
            title: "Hidden html inputs versus using a pathVariable in the URL of a PUT HTTP request",
            parentId: null,
            acceptedAnswerId: 18758895,
            creationDate: DateTime.Parse("2013-09-06T15:29:20"),
            body: "<p>My application currently <strong>makes massive use of hidden inputs</strong> such as:</p>      <pre><code>&lt;input type=\"hidden\" name=\"id\" value=\"123456\"/&gt;      </code></pre>      <p>For instance, using the above hidden input <strong>I can keep the id/pk of my object without displaying it to the end user</strong>.</p>      <p>I feel this <strong>goes against the principles of REST</strong> and I would like to know whether using a <strong>PUT url</strong> such as:</p>      <p><code>/advertisement/childminder/123456/edit</code> </p>      <p>and doing away with the hidden input altogether <strong>might not be a better idea</strong>...</p>      <p>Can anyone please advise?</p>      <p><strong>edit</strong>: <em>I have edited my question and changed from POST to PUT because I am dealing with modification and not creation.</em></p>",
            tags: "<rest><spring-mvc><put>"
            );

            posts.Add(
            id: 18658183,
            title: "Amazon ec2 for hosting java RESTful service with MongoDB as datastore",
            parentId: null,
            acceptedAnswerId: null,
            creationDate: DateTime.Parse("2013-09-06T12:44:50"),
            body: "<p>I want to host Java RESTful web service which operates over MongoDB. I am thinking of hosting it in Amazon EC2. Please suggest me what configuration I should use. Here is my systems abstract</p>      <ul>      <li>It is a set of java RESTful api which will be consumed by a mobile      application say XXX (in all platforms).</li>      <li>XXX is a kind of social networking app focused on user's movie      preference. A user can search a movie, view its profile write review,      like,comment others reviews, follow users,etc,.</li>      <li>I have all movie related information, user profile information and      reviews stored in MongoDB. Now with only 100 test users and 1L      movie's information my db size is 0.5 GB.</li>      <li>I must be capable of supporting large number of requests with low      latency.      <br></li>      </ul>      <p>I understand that I can increase RAM &amp; local storage whenever I want. But can I run mongoDB within the same EC2 instance? Want guidance on deciding combinations such as EBS , AWS  Elastic Beanstalk , etc. Please help me with choosing the right configuration.(I would also like to know other better options for hosting my webservice)</p>",
            tags: "<java><mongodb><rest><amazon-web-services><amazon-ec2>"
            );

            posts.Add(
            id: 18658020,
            title: "Parsing and encoding URL before sending to Rest webservice",
            parentId: null,
            acceptedAnswerId: 0,
            creationDate: DateTime.Parse("2013-09-06T12:34:57"),
            body: "<p>I am developing an App where I am sending get request to REST web service using      <code>@RequestMapping(value = \"myURLpattern\", method = RequestMethod.GET, produces = \"image/png\")</code></p>      <p>I came across a bug if somebody enters # in the URL query String it breaks from there      <code>http://myserver.com:8080/csg?x=1&amp;y=2#4&amp;z=32&amp;p=1</code></p>      <p>So I tried to encode it via filter <code>HTTPRequestWrapper</code> so that <code>#</code> will be replaced by <code>%23</code> using <code>URLEncoder</code>.</p>      <p>My problem is for encoding URL, first I need to read the <code>URL(request.getRequestURL())</code>.      and getURL again can't read string after #.      <code>request.getRequestURL()</code> will return only <code>(http://myserver.com:8080/csg?x=1&amp;y=2)</code> </p>      <p>Is there any other way to parse the complete url and encode it before sending to REST web service?</p>",
            tags: "<java><rest><url-encoding>"
            );

            posts.Add(
            id: 18655257,
            title: "How to send cookies on subsequent requests",
            parentId: null,
            acceptedAnswerId: null,
            creationDate: DateTime.Parse("2013-09-06T10:11:11"),
            body: "<p>I have a service running on a tomcat server that returns a <code>Response</code> object with <code>Set-Cookie</code> header set with a key-value pair. When i query that url directly by entering the service URL in the browser, the cookie gets stored and upon concurrent queries, the cookie info is sent to the server. Now, when i try to make that service request thro XMLHTTPRequest by providing that same URL, the cookie info is not stored. The js code is given below</p>      <pre><code> function loadXMLDoc() {      var xmlhttp;      if (window.XMLHttpRequest) {      xmlhttp = new XMLHttpRequest();      }      xmlhttp.onreadystatechange = function () {      if (xmlhttp.readyState == 4 &amp;&amp; xmlhttp.status == 200) {      document.getElementById(\"myDiv\").innerHTML = xmlhttp.responseText;      }      }      xmlhttp.open(\"GET\", url, true);      xmlhttp.send();      }      </code></pre>      <p>The Header info is as follows</p>      <pre><code>Request URL: url //removed the actual URL      Request Method:GET      Status Code:200 OK      Request Headersview source      Accept:*/*      Accept-Encoding:gzip,deflate,sdch      Accept-Language:en-US,en;q=0.8      Connection:keep-alive      Host:192.168.11.11:8080      Origin:null      User-Agent:Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/29.0.1547.62 Safari/537.36      Response Headersview source      Access-Control-Allow-Credentials:true      Access-Control-Allow-Headers:Origin, Content-Type, Accept, X-Json, Cookie      Access-Control-Allow-Methods:GET, POST, DELETE, PUT, OPTIONS      Access-Control-Allow-Origin:*      Content-Type:application/json      Date:Fri, 06 Sep 2013 10:04:49 GMT      Server:Apache-Coyote/1.1      Set-Cookie:key=value;Version=1 //removed actual cookie content      Transfer-Encoding:chunked      </code></pre>      <p>What am i missing here that makes the cookie go missing when requested thro XHR calls?.</p>      <p>Thanks in advance</p>      <p>Edit 1: I foundn out that in the second scenario (calling it thro javascript xmlHttpRequest), the cookie is not stored in the browser itself. Any pointers on why this might be happening. When i enter the service URL directly in the browser, the cookies are stored. Thro xmlHttpRequest, not stored. Is there something that I'm missing in either the Request or Response header?</p>      <p>Edit 2: Adding the Java class and the web.xml as well, so that there is more clarity.</p>      <pre><code>package com.gk.rest;      import javax.ws.rs.CookieParam;      import javax.ws.rs.GET;      import javax.ws.rs.Path;      import javax.ws.rs.Produces;      import javax.ws.rs.core.MediaType;      import javax.ws.rs.core.NewCookie;      import javax.ws.rs.core.Response;      import com.sun.jersey.spi.container.ContainerRequest;      import com.sun.jersey.spi.container.ContainerResponse;      import com.sun.jersey.spi.container.ContainerResponseFilter;      @Path(\"/rest\")      public class RestJersey implements ContainerResponseFilter {      @Path(\"/response\")      @GET      @Produces(MediaType.TEXT_PLAIN)      public Response sendResponse(@CookieParam(value = \"userId\") String userId ){      return Response.ok(\"Something\").cookie(new NewCookie(\"userId\",\"cookie1\")).build();      }      @Override      public ContainerResponse filter(ContainerRequest creq,      ContainerResponse cres) {      cres.getHttpHeaders().add(\"Access-Control-Allow-Origin\", \"*\");      cres.getHttpHeaders()      .add(\"Access-Control-Allow-Headers\",      \"origin, content-type, accept, cookie, x-json\");      cres.getHttpHeaders().add(\"Access-Control-Allow-Credentials\", \"true\");      cres.getHttpHeaders().add(\"Access-Control-Allow-Methods\",      \"GET, POST, PUT, DELETE, OPTIONS, HEAD\");      cres.getHttpHeaders().add(\"Access-Control-Max-Age\", \"1209600\");      return cres;      }      }      </code></pre>      <p>WEB.xml</p>      <pre><code>&lt;?xml version=\"1.0\" encoding=\"UTF-8\"?&gt;      &lt;web-app xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"http://java.sun.com/xml/ns/javaee\" xmlns:web=\"http://java.sun.com/xml/ns/javaee/web-app_2_5.xsd\" xsi:schemaLocation=\"http://java.sun.com/xml/ns/javaee http://java.sun.com/xml/ns/javaee/web-app_3_0.xsd\" id=\"WebApp_ID\" version=\"3.0\"&gt;      &lt;display-name&gt;RestJersey&lt;/display-name&gt;      &lt;servlet&gt;      &lt;servlet-name&gt;RestJersey&lt;/servlet-name&gt;      &lt;servlet-class&gt;com.sun.jersey.spi.container.servlet.ServletContainer&lt;/servlet-class&gt;      &lt;init-param&gt;      &lt;param-name&gt;com.sun.jersey.config.property.packages&lt;/param-name&gt;      &lt;param-value&gt;com.gk.rest&lt;/param-value&gt;      &lt;/init-param&gt;      &lt;init-param&gt;      &lt;param-name&gt;com.sun.jersey.spi.container.ContainerResponseFilters&lt;/param-name&gt;      &lt;param-value&gt;com.gk.rest.RestJersey&lt;/param-value&gt;      &lt;/init-param&gt;      &lt;load-on-startup&gt;1&lt;/load-on-startup&gt;      &lt;/servlet&gt;      &lt;servlet-mapping&gt;      &lt;servlet-name&gt;RestJersey&lt;/servlet-name&gt;      &lt;url-pattern&gt;/*&lt;/url-pattern&gt;      &lt;/servlet-mapping&gt;      &lt;/web-app&gt;      </code></pre>",
            tags: "<javascript><rest><cookies><xmlhttprequest><jersey>"
            );

            posts.Add(
            id: 18654677,
            title: "Is it correct to use Post instead of Get to fetch data in Web API",
            parentId: null,
            acceptedAnswerId: null,
            creationDate: DateTime.Parse("2013-09-06T09:41:08"),
            body: "<p>I am currently creating Restful API through ASP.Net WebAPI technology. I have 2 questions related to WebAPI</p>      <p>I had done below:</p>      <ol>      <li><p>Created below method in Controller class:</p>      <blockquote>      <p>public HttpResponseMessage PostOrderData(OrderParam OrderInfo)</p>      </blockquote></li>      <li>Based on Parameter OrderInfo, Query the SQL Server and get list of orders.</li>      <li>Set the Response.Content with the collection object:  </li>      </ol>      <blockquote>      <pre><code>List&lt;Orders&gt; ordList = new List&lt;Orders&gt;();      //filled the ordList from SQL query result      var response = Request.CreateResponse&lt;List&lt;Orders&gt;&gt;(HttpStatusCode.OK, ordList);      </code></pre>      </blockquote>      <ol>      <li>On Client side,  </li>      </ol>      <blockquote>      <pre><code>OrderParam ordparam = new OrderParam();      response = client.PostAsJsonAsync(\"api/order\", ordparam).Result;      if (response.IsSuccessStatusCode)      {      List&lt;Orders&gt; mydata = response.Content.ReadAsAsync&lt;List&lt;Orders&gt;&gt;().Result;      }      </code></pre>      </blockquote>      <p>So question: is it fine to Post the data to server to Get the data i.e. usage of Post data insted of Get is correct? Is there any disadvantage in approach? (One disadvantage is: I will not able to query directly from browser) I have used Post here because parameter \"OrderParam\" might extend in future and there might be problem due to increase in Length of URL due to that. </p>      <p>2nd Question is: I have used classes for parameter and for returning objects i.e. OrderParam and Orders. Now consumer (clients) of this web api are different customers and they will consume API through .Net (C#) or through Jquery/JS. So do we need to pass this class file containing defination of OrderParam and Orders classes manually to each client? and send each time to client when there will be any change in above classes?</p>      <p>Thanks in advance<br>      Shah</p>",
            tags: "<rest><asp.net-web-api><web-api>"
            );

            posts.Add(
            id: 18654496,
            title: "How to call json rest service in html through dojo",
            parentId: null,
            acceptedAnswerId: null,
            creationDate: DateTime.Parse("2013-09-06T09:31:24"),
            body: "<p>I am developing an maven web application using rest web services. I am trying to call the simple web service from the dojo. But I  did not know getting started to call rest service through dojo. My web service code is:</p>      <pre><code>@GET      @Path(\"/users\")      @Produces(\"application/json\")      public ArrayList dynamicFetch() {      ArrayList&lt;User&gt; ar = new ArrayList&lt;User&gt;();      User u1 = new User(1,\"Test\",30);      ar.add(u1);      u1 = new User(2,\"test2\",31);      ar.add(u1);      return ar;      }      </code></pre>      <p>Which executes and shows</p>      <pre><code>[{\"age\":30,\"name\":\"Test\",\"id\":1},{\"age\":31,\"name\":\"test2\",\"id\":2}]      </code></pre>      <p>How can i call this json object in html through dojo since all my elements are dojo..?</p>      <p>Please Help any help will be apprciated more</p>      <p>Thanks</p>",
            tags: "<javascript><json><rest><maven><dojo>"
            );

            posts.Add(
            id: 18650657,
            title: "Best practices for managing esb configurations",
            parentId: null,
            acceptedAnswerId: null,
            creationDate: DateTime.Parse("2013-09-06T05:38:59"),
            body: "<p>We have a web application that exposes lot of REST Service API.  We have another application that consume these REST services through ws02esb.  We have maintained all synapse configuration in several xml [one for each rest service], this is not helping us in maintaining our configuration better (or) creating lot of issues while promoting to QA</p>      <p>After browsing through, we are thinking of managing them through wso2 developer studio, and during the build process develop a car file that can be used to deploy on ESB.</p>      <p>As we are new to ws02 esb, would like to know </p>      <ol>      <li>what are the best practices for managing the esb synapse      configurations</li>      <li>how we can generate car using maven</li>      <li>we have a situation where in the service URLs are tenant based, how do we      utilise single CAR serving multiple tenants - requesting pointers</li>      <li>how certain URLs can be sourced from property resources</li>      </ol>",
            tags: "<rest><maven><wso2esb><wso2developerstudio>"
            );

            posts.Add(
            id: 18648882,
            title: "REST Authentication: put key in custom header or Authorization header?",
            parentId: null,
            acceptedAnswerId: null,
            creationDate: DateTime.Parse("2013-09-06T02:22:25"),
            body: "<p>I'm designing a REST API framework for a large security-conscious company.</p>      <p>Every caller to our services needs to provide a client key to access the system. We will use that for authorizing access for that particular client as well as rate limiting and monitoring. Also <em>some</em> of our API calls will access customer data, and we will use OAuth 2 tokens to control access to that data.</p>      <p>My question is how to pass the client key. I can not use HTTP Basic Authentication or a query parameter because we cannot pass it in the URI (URIs get logged sometimes) -- it must be in an HTTP header instead. So I have thought of two approaches, both with flaws:</p>      <p>(1) Invent our own header: <code>MyCompanyAPIKey: api-key-goes-here</code>. This is flawed because we will be inventing our own header and that's a poor design choice. It won't work with anyone else or with standard tools (because we invented our own).</p>      <p>(2) Use the Authorization header: <code>Authorization: Bearer api-key-goes-here</code>. This is flawed because it will conflict with OAuth (which needs that header) in the cases where we use that. Technically I suppose we don't <em>need</em> the client key when we have an OAuth token (since the OAuth token was specific to a single client), but I don't know if the normal tools can handle that.</p>      <p>How do you think we should proceed?</p>",
            tags: "<rest><restful-authentication>"
            );

            posts.Add(
            id: 18645877,
            title: "Spring MVC Rest: No mapping found for HTTP request with URI [/ecommerce-api/rest/checkout] in DispatcherServlet",
            parentId: null,
            acceptedAnswerId: 18647511,
            creationDate: DateTime.Parse("2013-09-05T20:59:43"),
            body: "<p><strong>(probably) solved</strong> <code>or at least it seems to be. I'm not really sure where it was the problem... For sure the config suggested by Biju Kunjummen it's working and seems to me cleaner. What I'm doing now to not generate mess is to work only inside Eclipse, sometime cleaning the projects and never using maven to package &amp; deploy it (at least during day to day programming, I guess with some robust maven script or CI Server everything will work fine too). Thanks again guys!</code></p>      <p>I'm trying to setup a Rest API with Spring MVC. I've read a lot of documentation but I'm still getting the error in the subject:</p>      <blockquote>      <p>No mapping found for HTTP request with URI [/ecommerce-api/rest/checkout] in DispatcherServlet</p>      </blockquote>      <p>The problem is exactly the same reported (and solved) in other similar questions, like this one <a href=\"http://stackoverflow.com/questions/15163035/fixed-no-mapping-found-trying-to-set-up-a-restfull-interface-using-spring-mvc\">FIXED: &quot;No mapping found&quot; Trying to set up a RESTfull interface using Spring-MVC</a></p>      <p>The really strange thing is that, without apparently changing anything in my code, sometime it works (hurray!) sometimes it doesn't (...). I'm pretty sure nothing changes between the two moments, since for example sometime I'm phisically away for a coffee or something then I come back and it stops working (it's exhausting...).</p>      <p>Whatever... at this particular moment, my code is the following:</p>      <p>web.xml</p>      <pre><code> &lt;?xml version=\"1.0\" encoding=\"UTF-8\"?&gt;      &lt;web-app version=\"2.5\" xmlns=\"http://java.sun.com/xml/ns/javaee\"      xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"      xsi:schemaLocation=\"http://java.sun.com/xml/ns/javaee http://java.sun.com/xml/ns/javaee/web-app_2_5.xsd\"&gt;      &lt;context-param&gt;      &lt;param-name&gt;contextConfigLocation&lt;/param-name&gt;      &lt;param-value&gt;      /WEB-INF/api-dispatcher-servlet.xml      &lt;/param-value&gt;      &lt;/context-param&gt;      &lt;servlet&gt;      &lt;servlet-name&gt;api-dispatcher&lt;/servlet-name&gt;      &lt;servlet-class&gt;      org.springframework.web.servlet.DispatcherServlet      &lt;/servlet-class&gt;      &lt;load-on-startup&gt;1&lt;/load-on-startup&gt;      &lt;/servlet&gt;      &lt;servlet-mapping&gt;      &lt;servlet-name&gt;api-dispatcher&lt;/servlet-name&gt;      &lt;url-pattern&gt;/rest/*&lt;/url-pattern&gt;      &lt;/servlet-mapping&gt;      &lt;listener&gt;      &lt;listener-class&gt;      org.springframework.web.context.ContextLoaderListener      &lt;/listener-class&gt;      &lt;/listener&gt;      &lt;listener&gt;      &lt;listener-class&gt;      org.springframework.web.context.request.RequestContextListener      &lt;/listener-class&gt;      &lt;/listener&gt;      &lt;/web-app&gt;      </code></pre>      <p>api-dispatcher-servlet.xml</p>      <pre><code>&lt;?xml version=\"1.0\" encoding=\"UTF-8\" ?&gt;      &lt;beans:beans xmlns=\"http://www.springframework.org/schema/mvc\"      xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:beans=\"http://www.springframework.org/schema/beans\"      xmlns:context=\"http://www.springframework.org/schema/context\"      xmlns:util=\"http://www.springframework.org/schema/util\"      xmlns:mvc=\"http://www.springframework.org/schema/mvc\"      xsi:schemaLocation=\"http://www.springframework.org/schema/mvc      http://www.springframework.org/schema/mvc/spring-mvc-3.0.xsd      http://www.springframework.org/schema/beans      http://www.springframework.org/schema/beans/spring-beans-3.0.xsd      http://www.springframework.org/schema/util      http://www.springframework.org/schema/util/spring-util-3.0.xsd http://www.springframework.org/schema/context      http://www.springframework.org/schema/context/spring-context-3.0.xsd\"&gt;      &lt;context:component-scan base-package=\"com.myapp.api.ecommerce.controller\" /&gt;      &lt;context:annotation-config /&gt;      &lt;mvc:annotation-driven /&gt;      &lt;/beans:beans&gt;      </code></pre>      <p>com.myapp.ecommerce.controller.CheckoutController</p>      <pre><code>package com.myapp.api.ecommerce.controller;      import org.springframework.stereotype.Controller;      import org.springframework.web.bind.annotation.RequestMapping;      import org.springframework.web.bind.annotation.RequestMethod;      import org.springframework.web.bind.annotation.ResponseBody;      import com.myapp.ecommerce.service.checkout.manager.CheckoutManager;      import com.myapp.ecommerce.service.checkout.manager.CheckoutManagerImpl;      import com.myapp.ecommerce.service.checkout.model.Checkout;      @Controller      @RequestMapping(\"/checkout\")      public class CheckoutController {      private CheckoutManager checkoutManager = new CheckoutManagerImpl();      @RequestMapping(method = RequestMethod.GET)      public @ResponseBody Checkout getCheckout() {      return checkoutManager.findById(\"514f2a8e20f7a78a1400001f\");      }      }      </code></pre>      <p>A snippet of the log file of the application server (VFabric but I've also tried with Tomcat 7) when I try to <em>GET ecommerce-api/rest/checkout</em>:</p>      <pre><code>2013-09-05 22:31:37,760 DEBUG [tomcat-http--5] servlet.DispatcherServlet (DispatcherServlet.java:823) - DispatcherServlet with name 'api-dispatcher' processing GET request for [/ecommerce-api/rest/checkout]      2013-09-05 22:31:37,763 DEBUG [tomcat-http--5] handler.AbstractHandlerMethodMapping (AbstractHandlerMethodMapping.java:220) - Looking up handler method for path /checkout      2013-09-05 22:31:37,763 DEBUG [tomcat-http--5] handler.AbstractHandlerMethodMapping (AbstractHandlerMethodMapping.java:230) - Did not find handler method for [/checkout]      2013-09-05 22:31:37,764 WARN  [tomcat-http--5] servlet.DispatcherServlet (DispatcherServlet.java:1108) - No mapping found for HTTP request with URI [/ecommerce-api/rest/checkout] in DispatcherServlet with name 'api-dispatcher'      2013-09-05 22:31:37,764 DEBUG [tomcat-http--5] servlet.FrameworkServlet (FrameworkServlet.java:966) - Successfully completed request      </code></pre>      <p>I really don't know what to do since it worked since last time I shut down my Mac and until that moment I thought to know how to do it...</p>      <p>thanks in advance!      Alexio</p>",
            tags: "<java><spring><rest><spring-mvc>"
            );

            posts.Add(
            id: 18643292,
            title: "How do I build service that accepts name and values through POST in c#?",
            parentId: null,
            acceptedAnswerId: null,
            creationDate: DateTime.Parse("2013-09-05T18:17:02"),
            body: "<p>I found out WCF does not accept POST requests. I know REST architecture does. What would  the process and the service look like for building something like this:      client code sends data to the server through POST method. </p>      <p>The data contains name=john&amp;age=20&amp;country=USA</p>      <p>and the service response is xml file content. I looked at WCF rest components but i am worried it required client code to reference the service. I do not want the client to add service reference. In fact service should not be aware of any technology the client is written in. All it cares is it is receiving above data through POST and it responds with xml file content.</p>",
            tags: "<c#><web-services><rest><soa>"
            );

            posts.Add(
            id: 18637906,
            title: "ASP.NET Web Api: Correct way to serve OData-queryable GET requests",
            parentId: null,
            acceptedAnswerId: 18639159,
            creationDate: DateTime.Parse("2013-09-05T13:38:31"),
            body: "<p>What is the right way to serve OData-queryable GET requests in ASP.NET Web Api? That may sound like a \"what is better\" question, but it should be a \"what does work\" question.</p>      <p>Some assumptions:</p>      <ul>      <li>To enable OData-querying, you have to put the <code>Queryable</code> attribute to the action method that returns <code>IQueryable&lt;Model&gt;</code>. Therefore, you have to expose the domain model?</li>      <li>The domain model uses Entity Framework 5 and has navigation properties. The XML and Json Serializers do not like the EF proxies, so you have to disable them for OData queries?</li>      <li>The serializers pick up the navigation properties and serve them to the user.</li>      </ul>      <p>So if I have a <code>Category</code> type that has navigation properties for parent and children, the serializers complain that I have cyclic references, and I cannot get rid of this error.</p>      <p>I have read that I should use DTOs, but HOW? How can I provide a <code>IQueryable&lt;DTOModel&gt;</code> to the user that will create the appropriate SQL for the database? Remember, I want to use <code>$filter</code> and the like.</p>      <p>I just want to give the user a filterable list of Model objects without the serialized navigation properties.... but HOW?</p>",
            tags: "<json><asp.net-web-api><entity-framework-5><odata>"
            );

            posts.Add(
            id: 18637891,
            title: "RESTful Authentication via spring with username/passwd and token",
            parentId: null,
            acceptedAnswerId: null,
            creationDate: DateTime.Parse("2013-09-05T13:37:51"),
            body: "<p>We are developing a mobile app. The server side is REST using Spring 3 MVC. I was trying to integrate the Spring Security with it to secure resources. I went through lot of online material to get the information on how to do it. I understand the architecture however when it comes to implementing I am still confused. I referred a question asked here, <a href=\"http://stackoverflow.com/questions/10826293/restful-authentication-via-spring\">RESTful Authentication via Spring</a>. And we are having the same requirements. I understood the code however I am confused with the part where first authenticate request comes in, at that time token will not be present as the part of the header. So the same filter won't work. So I was wondering how should I implement it. Please correct me if I am wrong. I was thinking of implementing it as follows:</p>      <ol>      <li>A separate filter that authenticates user using username password from the request.</li>      <li>After authentication the filter sets the authentication info in the context.</li>      <li>And another filter that works with tokens for authentication for all API URLS.</li>      </ol>      <p>Is this the correct way to implement it? Any help is appreciated.</p>      <p>Regards,</p>      <p>Meghana </p>",
            tags: "<rest><authentication><token><username>"
            );

            posts.Add(
            id: 18637493,
            title: "Update multilevel records using - wcf client data services / odata",
            parentId: null,
            acceptedAnswerId: null,
            creationDate: DateTime.Parse("2013-09-05T13:20:00"),
            body: "<p>I am looking for service example where multilevel update is done by using wcf client data service.</p>      <p>For example, I have Order 101 inside this class I have list of Order Details 301,302,303.</p>      <p>I want to send complete Order with all order details to server for update operation.</p>",
            tags: "<c#><wcf-data-services><odata>"
            );

            posts.Add(
            id: 18633850,
            title: "Consuming REST service with C# code",
            parentId: null,
            acceptedAnswerId: 18655311,
            creationDate: DateTime.Parse("2013-09-05T10:26:58"),
            body: "<p>I am using the following code to get the json result from the service. It works fine for <code>get</code> methods. But when the method type is <code>POST</code> the request address changes to the previous address.</p>      <p>ie;</p>      <p>on the first call to this method the request.address=<code>XXXXX.com:1234/xxx/oldv1.json</code> (method type is <code>get</code>)</p>      <p>and it returns a json string from which I extract another address:<code>XXXXX.com:1234/xxx/newv1.json</code>      and now I call the makerequest method with this endpoint and method type POST, contenttype=\"application/x-www-form-urlencoded\".</p>      <p>When I put breakpint at <code>using (var response = (HttpWebResponse)request.GetResponse())</code> and checked the request.address value, it was <code>XXXXX.com:1234/xxx/newv1.json</code></p>      <p>But after that line is executed, the address changes to <code>XXXXX.com:1234/xxx/oldv1.json</code> and the function returns the same response I got with the first Endpoint(<code>XXXXX.com:1234/xxx/oldv1.json</code>).      Can anybody tell what I am doing wrong here?</p>      <p>Is there any better method to consume the service with POST method?</p>      <pre><code>public string MakeRequest(string EndPoint,string Method, string contentType)      {      var request = (HttpWebRequest)WebRequest.Create(EndPoint);      request.Method = Method;      request.ContentLength = 0;      request.ContentType =contentType;      if ( Method == HttpVerb.POST)      {      var encoding = new UTF8Encoding();      var bytes = Encoding.GetEncoding(\"iso-8859-1\").GetBytes(\"username=123&amp;password=123\");      request.ContentLength = bytes.Length;      using (var writeStream = request.GetRequestStream())      {      writeStream.Write(bytes, 0, bytes.Length);      }      }      using (var response = (HttpWebResponse)request.GetResponse())// request.address changes at this line on \"POST\" method types      {      var responseValue = string.Empty;      if (response.StatusCode != HttpStatusCode.OK)      {      var message = String.Format(\"Request failed. Received HTTP {0}\", response.StatusCode);      throw new ApplicationException(message);      }      // grab the response      using (var responseStream = response.GetResponseStream())      {      if (responseStream != null)      using (var reader = new StreamReader(responseStream))      {      responseValue = reader.ReadToEnd();      }      }      return responseValue;      }      </code></pre>      <p><strong>EDIT:</strong> Yesterday I asked <a href=\"http://stackoverflow.com/questions/18618243/how-to-receive-json-result-using-jquery-ajax\">THIS Question</a> about consuming the service at client side and many suggested it needs to be done at server side as the other domain might not allow accessing the json result at client side. </p>",
            tags: "<c#><json><rest>"
            );

            posts.Add(
            id: 18633532,
            title: "Hateoas java.lang.IllegalStateException: Could not find current request via RequestContextHolder",
            parentId: null,
            acceptedAnswerId: null,
            creationDate: DateTime.Parse("2013-09-05T10:11:11"),
            body: "<p>I am using Hateoas feature for calling Controller's post method url. But i am getting above exception when I try to use linkTo method of ControllerLinkBuilder class as described below:</p>      <p><strong>Java class:</strong></p>      <pre><code>import static org.springframework.hateoas.mvc.ControllerLinkBuilder.linkTo;      import org.springframework.stereotype.Component;      @Component      public class CallController{      public String doThis()      {      ManagementResource resource = new ManagementResource();      resource.add(linkTo(DataController.class).withRel(\"postData\"));      return \"\";      }      }      </code></pre>      <p><strong>Controller Class</strong></p>      <pre><code>@Controller      @RequestMapping(\"/data\")      public class DataController {      @RequestMapping(method = RequestMethod.POST, value = \"\")      public ResponseEntity&lt;ManagementResource&gt; postData(@RequestBody Data1 data) {      RSResponse&lt;Data1&gt; response = new RSResponse&lt;Data1&gt;();      response.setStatus(RSResponse.Status.SUCCESS);      response.setData(data);      return new ResponseEntity&lt;ManagementResource&gt;(HttpStatus.CREATED);      }      }      </code></pre>      <p>If anyone has idea about it, what I'm doing wrong here, please tell me. Thanks in advance.</p>",
            tags: "<spring><rest><jsf><hateoas>"
            );

            posts.Add(
            id: 18631361,
            title: "Add Attachment to Jira via REST API",
            parentId: null,
            acceptedAnswerId: 18656723,
            creationDate: DateTime.Parse("2013-09-05T08:27:51"),
            body: "<p>I'm trying to post an attachment o JIRA using the latest REST API.      Here's my code:</p>      <pre><code>public boolean addAttachmentToIssue(String issueKey, String path){      String auth = new      String(org.apache.commons.codec.binary.Base64.encodeBase64((user+\":\"+pass).getBytes()));      Client client = Client.create();      WebResource webResource = client.resource(baseURL+\"issue/\"+issueKey+\"/attachments\");      FormDataMultiPart formDataMultiPart = new FormDataMultiPart();      File f = new File(path);      if(f.exists() &amp;&amp; f.isFile()){      FileInputStream fis = null;      try {      fis = new FileInputStream(f);      } catch (FileNotFoundException e) {      return false;      }      ByteArrayOutputStream bos = new ByteArrayOutputStream();      byte[] buf = new byte[1024];      try {      for (int readNum; (readNum = fis.read(buf)) != -1;) {      bos.write(buf, 0, readNum); //no doubt here is 0      }      fis.close();      bos.close();      } catch (IOException ex) {      try {      fis.close();      bos.close();      } catch (IOException e) {      return false;      }      return false;      }      byte[] bytes = bos.toByteArray();      FormDataBodyPart bodyPart = new FormDataBodyPart(\"file\", new ByteArrayInputStream(bytes), MediaType.APPLICATION_OCTET_STREAM_TYPE);      formDataMultiPart.bodyPart(bodyPart);      }else{      return false;      }      ClientResponse response = null;      response = webResource.header(\"Authorization\", \"Basic \" + auth).header(\"X-Atlassian-Token\", \"nocheck\").type(MediaType.MULTIPART_FORM_DATA).accept(\"application/json\").post(ClientResponse.class, formDataMultiPart);      System.out.println(response);      int statusCode = response.getStatus();      System.out.println(statusCode);      String resp = response.getEntity(String.class);      System.out.println(resp);      return true;      }      </code></pre>      <p>However, i get the following response:</p>      <pre><code>POST http://localhost:8082/rest/api/2/issue/TEST-2/attachments returned a response status of 404 Not Found      404      XSRF check failed      </code></pre>      <p>An Issue with key TEST-2 does exist in the my local JIRA instance and I can add the attachment \"by hand\" in the Jira app itself.      I know that i must add a header of type \"X-Atlassian-Token:nocheck\" to prevent XSRF, but, by the output, I must be doing something wrong..      What confuses me even further is that a 404 is thrown after the XSRF check failed.</p>      <p>I've scavenged google for answers with no success      Can anyone hazard a guess to what I'm doing wrong?</p>",
            tags: "<java><rest><jira><attachment>"
            );

            posts.Add(
            id: 18631181,
            title: "How to save same select name using backbone.js and rest webservice",
            parentId: null,
            acceptedAnswerId: null,
            creationDate: DateTime.Parse("2013-09-05T08:19:02"),
            body: "<p>I have a  3 select tag with the same name and I want to update my table in one saving only using backbone.js.      In my table there is 3 fix record : Theres no add, delete only update query is using in this code.</p>      <pre><code>id   docs_id      1      5      2      3      3      6      </code></pre>      <p>Here are my html code:</p>      <pre><code>&lt;select name=\"docs_id\" id=\"docs_id\"  &gt;      &lt;option value=\"0\"&gt;&amp;nbsp;&lt;/option&gt;      &lt;/select&gt;      &lt;select name=\"docs_id\" id=\"docs_id\"  &gt;      &lt;option value=\"0\"&gt;&amp;nbsp;&lt;/option&gt;      &lt;/select&gt;      &lt;select name=\"docs_id\" id=\"docs_id\"  &gt;      &lt;option value=\"0\"&gt;&amp;nbsp;&lt;/option&gt;      &lt;/select&gt;      &lt;div class=\"modal-footer\"&gt;      &lt;a href=\"#\" class=\"btn\" data-dismiss=\"modal\"&gt;Cancel&lt;/a&gt;      &lt;a href=\"#\" class=\"btn btn-success\" id=\"options-submit\"&gt;Submit&lt;/a&gt;      &lt;/div&gt;      </code></pre>      <p>In my directory view javascript I have this code</p>      <pre><code>events: {      \"click .record-set\"   : \"setDocs\"      },      setDocs: function (e) {      e.preventDefault();      var that = this;      $('#setDocs').live('show', function () {      $(this).find('.btn-success').die().live('click', function () {      var d = Array;      $.map($('.inopts select').serializeArray(), function(n, i) {      d[n['name']] = n['value'];      });      options2 = new Options2();      options2.save(d, {      success: function (model, response) {      this.$el = $('#setDocs');      this.$el.modal('hide');      alert = new AlertView({type: 'success', message: 'New record successfully added.'});      alert.render();      },      error: function (model, response) {      alert = new AlertView({type: 'error', message: response});      alert.render();      }      });      });      })      .modal();      },      .............      </code></pre>      <p>Here is my model javacript</p>      <pre><code>var Options2 = Backbone.Model.extend({      idAttribute: 'id',      url: function () {      if (this.isNew()) {      return BASE_URL + 'api/document_various';      } else {      return BASE_URL + 'api/document_various/id/' + this.get('id');      }      },      defaults: {      docs_id: '',      }      </code></pre>      <p>});</p>      <p>In my rest API dis my code</p>      <pre><code> private function _document_various_save($db, $mode)      {      $response = array('status' =&gt; 'failed', 'message' =&gt; '');      $db-&gt;docs_id        = $this-&gt;{$mode}('docs_id');      $id = $db-&gt;save();      if ($id) {      $response['id'] = $id;      $response['status']  = 'success';      } else {      $response['message'] = $db-&gt;get_validation_errors();      }      return $response;      }      </code></pre>",
            tags: "<php><rest><backbone.js>"
            );

            posts.Add(
            id: 18631091,
            title: "How to call web API in WPF 4.0",
            parentId: null,
            acceptedAnswerId: null,
            creationDate: DateTime.Parse("2013-09-05T08:14:14"),
            body: "<p>I wants to <strong>call web API in my WPF 4.0 application</strong> , where API receive request in JSON format &amp; send response in JSON format.</p>      <p>I got the solution to <strong>call web API in WPF 4.5</strong> from here <a href=\"http://www.asp.net/web-api/overview/web-api-clients/calling-a-web-api-from-a-wpf-application\" rel=\"nofollow\">http://www.asp.net/web-api/overview/web-api-clients/calling-a-web-api-from-a-wpf-application</a></p>      <p>but <strong>i want same kind of solution in WPF 4.0</strong></p>      <p>please help me</p>",
            tags: "<asp.net><.net><json><rest><wpf-4.0>"
            );

            posts.Add(
            id: 18630055,
            title: "Get Jenkins artifacts URl from REST api",
            parentId: null,
            acceptedAnswerId: null,
            creationDate: DateTime.Parse("2013-09-05T07:16:45"),
            body: "<p>Is there any way to get artifacts download url from jenkins <strong>REST API</strong>. I am using <code>artifact deployer</code> and atrifacts can be download from  </p>      <pre><code>http://localhost:8080/job/jobname/buildId/deployedartifact/downloads/artifacts.{id}      </code></pre>      <p>Is it possible to get the url infomation from REST api??</p>",
            tags: "<rest><jenkins>"
            );

            posts.Add(
            id: 18628789,
            title: "Flask - implement a batch interface",
            parentId: null,
            acceptedAnswerId: null,
            creationDate: DateTime.Parse("2013-09-05T05:59:08"),
            body: "<p>I have used flask to implement a restful api?</p>      <p>To reduce the quantity of requests from ios app, i want to implement a batch interface.      It looks like this:</p>      <pre><code>method: post      uri: /app/batch      data:[json]      {      \"on-fail\": \"ignore|stop\",      \"requests\": [      {      \"method\": \"get\",      \"uri\": \"/user/~me\",      \"args\": {      \"arg1\": \"value1\",      \"arg2\": \"value2\"      }      },      {      \"method\": \"put\",      \"uri\": \"/user/~me/oauth/QZONE\",      \"args\": {      \"arg1\": \"value1\",      \"arg2\": \"value2\",      }      }      ]      }      return:[json]      {      \"responses\": [      {      \"method\": \"get\",      \"uri\": \"/user/~me\",      \"status_code\": \"200\",      \"body\": { ... }      },      {      \"method\": \"put\",      \"uri\": \"/user/~me/oauth/QZONE\",      \"status_code\": \"200\",      \"body\": { ... }      }      ]      }      </code></pre>      <p>So I need to call the sub requests' handler inside the <code>batch</code> request context, and set the request's args manually?Is there some appropriate way to do that?</p>      <p>I known <code>werkzeug.test.Client</code> do the things, but I'm not sure I can use it in production environment instead of testing environment.</p>",
            tags: "<python><rest><flask>"
            );

            posts.Add(
            id: 18627731,
            title: "Return Create Object ID Django Rest Framework",
            parentId: null,
            acceptedAnswerId: 18644588,
            creationDate: DateTime.Parse("2013-09-05T04:25:44"),
            body: "<p>I have a view that extends CreateAPIView. I'd like the ID of the created object included somewhere in the response. How can I accomplish this?</p>",
            tags: "<python><django><rest>"
            );

            posts.Add(
            id: 18626676,
            title: "Is Emberjs (with Ember Data) and Django (with rest framework) ready for prime time?",
            parentId: null,
            acceptedAnswerId: null,
            creationDate: DateTime.Parse("2013-09-05T02:12:21"),
            body: "<p>I have been studying emberjs and tried to get it to work with the django rest framework, without much luck.</p>      <p>Here is what I have found:</p>      <ol>      <li>django rest framework does not natively spit the json format that Ember expects</li>      <li>ember django rest framework adapter is based on ember-data, which leads to next point</li>      <li>ember-data is not production ready, and the rest framework adapter does not work on the latest ember-data</li>      <li>someone mentioned to roll your own without using ember data in this link <a href=\"http://discuss.emberjs.com/t/ember-data-endless-frustration/893/2\" rel=\"nofollow\">http://discuss.emberjs.com/t/ember-data-endless-frustration/893/</a>2 but it makes me feel like doing things twice since I already defined my models in django.</li>      </ol>      <p>My question is, is this combination ready for prime time and has anyone used this combination for any production sites?</p>",
            tags: "<django><rest><data><frameworks><ember.js>"
            );

            posts.Add(
            id: 18622678,
            title: "REST API - use endpoint properties in authorization model?",
            parentId: null,
            acceptedAnswerId: 18640223,
            creationDate: DateTime.Parse("2013-09-04T19:55:30"),
            body: "<p>Is it a good practice to give permission to access endpoint?</p>      <p>For example</p>      <pre><code>POST /permissions {method: \"GET\", resource: {href: \"/users/*\"}}      -&gt; 201 {href: \"/permissions/12345\", id: 12345}      POST /roles/123/rolePermissions {permission: {id: 12345}}      </code></pre>      <p>and after this check the permission with the given pattern...</p>      <p>For example if I want to give permission to a friend to edit one of my articles, I can do the following:</p>      <pre><code>GET /users/13/userPermissions      -&gt; 200 {items: [{id: 99, shares: [], permission: {id: 1234, method: \"PUT\", resource: {href: \"/article/1\"}}}, ...]}      </code></pre>      <p>The client prints a fancy table with my custom permissions, now I can choose the permission 1234, and share it with my friend:</p>      <pre><code>POST /userPermissions/99/shares {user: {id: 15}}      -&gt; 201 {id: 111111}      -&gt; new permission to \"DELETE /userPermissions/99/shares/111111\" is created and given to me (13)      -&gt; permission to \"PUT /article/1\" given to my friend (15)      </code></pre>      <p>and after that I can delete it too</p>      <pre><code>DELETE /userPermissions/99/shares/11111      -&gt; permission to \"PUT /article/1\" revoked from my friend (15)      -&gt; permission to \"DELETE /userPermissions/99/shares/111111\" revoked from me (13) and deleted      </code></pre>      <p>If this approach is not okay to store and check permissions, then what are the best practices?</p>",
            tags: "<api><rest>"
            );

            posts.Add(
            id: 18621401,
            title: "Siutable framework for Async, REST and JSON WebService",
            parentId: null,
            acceptedAnswerId: 0,
            creationDate: DateTime.Parse("2013-09-04T18:39:36"),
            body: "<p>I found <a href=\"http://stackoverflow.com/questions/4693434/what-is-the-best-web-language-framework-to-use-with-a-rest-api-and-json\">similar question</a> but there's no satisfying answer for me.</p>      <p>I'm going to create some start-up web application and I'm looking for <strong>the most suitable</strong> language and framework.</p>      <p>What I've decided is that:</p>      <ul>      <li>UI has to be separated from backend</li>      <li>Frontend will receive data in <strong>JSON</strong></li>      <li>Backend need to be <strong>REST</strong>-ful</li>      <li>Data will be stored in <strong>MongoDB</strong></li>      <li>It will be deployed initially on free cloud hosting</li>      </ul>      <p>I have no preference for any particular language, I only need framework that:</p>      <ul>      <li>has comprehensive documentation</li>      <li>has strong community support</li>      <li>doesn't require any special measures to implement REST access</li>      <li>can produce JSON responses in easy way</li>      </ul>      <p>So what I'm basically looking for is web framework, which supports creating scalable web application and provides easy implementation (on language and technology level). I'm aware that there isn't anything like <em>the best language</em>, but I also know that my requirements are rather strict and there can be some choice from for example:</p>      <ul>      <li>EJB</li>      <li>Ruby On Rails</li>      <li>Django</li>      <li>Spring</li>      <li>Play!</li>      <li>Grails</li>      </ul>      <p>Could you (as experienced programmers) present me advantages and drawbacks of web frameworks that you were working with?</p>      <hr>      <p>OK. Many thanks for your help. From my investigation I abstracted the following frameworks:</p>      <ul>      <li>Ruby on Rails</li>      <li>Play</li>      <li>Node.js</li>      </ul>      <p>After a deep research I know that what I really want is <strong>WebService</strong> which has access to some data from DB and provides it in JSON to UI side to present it to a client.      I also know that my application will be rather big, so I need robust and comprehensive web framework, but I still don't know which one to choose.</p>",
            tags: "<json><web-services><mongodb><rest><web-frameworks>"
            );

            posts.Add(
            id: 18620224,
            title: "RESTful Api or direct access?",
            parentId: null,
            acceptedAnswerId: 0,
            creationDate: DateTime.Parse("2013-09-04T17:28:52"),
            body: "<p>I'm asking this question because some of the websites is visited seems to be using a RESTful API to access the data even if it's on the website...</p>      <p>For example: my website will have 6 pages and 5 of them use the DB. But, I will also have a REST api for my partners...</p>      <p>So, the question is:</p>      <blockquote>      <p><strong>On my website, is it better to access directly the DB via <code>mysqli_query</code> or to use a RESTful API with Ajax calls to load data?</strong></p>      </blockquote>      <p>Just a note: I'll be using Zend Framework 2 for my RESTful API except if someone has a better option... I know Node.js and PHP... I'm able to write it in Ruby or something if it's better for me... Need a opinion on that...</p>",
            tags: "<php><mysql><ajax><rest><website>"
            );

            posts.Add(
            id: 18619151,
            title: "Backbone RESTful JSON schema",
            parentId: null,
            acceptedAnswerId: 18653034,
            creationDate: DateTime.Parse("2013-09-04T16:26:05"),
            body: "<p>If I want to present the following data to a backbone.js collection, what should the JSON that I create look like?:</p>      <pre><code>id: 1      fname: \"John\"      surname: \"Lennon\"      email: \"john@beatles.com\"      age: 22      id: 2      fname: \"Paul\"      surname: \"McCartney\"      email: \"paul@beatles.com\"      age: 22      id: 3      fname: \"George\"      surname: \"Harrison\"      email: \"george@beatles.com\"      age: 20      id: 4      fname: \"Ringo\"      surname: \"Starr\"      email: \"ringo@beatles.com\"      age: 24      </code></pre>      <p>I have been exporting it as follows:</p>      <pre><code>[{      \"id\":1,      \"fname\":\"John\",      \"surname\":\"Lennon\",      \"email\":\"john@beatles.com\",      \"age\":22      },{      \"id\":2,      \"fname\":\"Paul\",      \"surname\":\"McCartney\",      \"email\":\"paul@beatles.com\",      \"age\":22      },{      \"id\":3,      \"fname\":\"George\",      \"surname\":\"Harrison\",      \"email\":\"george@beatles.com\",      \"age\":20      },{      \"id\":4,      \"fname\":\"Ringo\",      \"surname\":\"Starr\",      \"email\":\"ringo@beatles.com\",      \"age\":24      }]      </code></pre>      <p>When presented with the JSON above, my collection only seems to contain the final Beatle (Ringo).</p>      <hr>      <p><strong>This is my View:</strong></p>      <pre><code>var app = app || {};      app.BeatleView = Backbone.View.extend({      el: '#page',      template: Handlebars.getTemplate( 'account_statement' ),      initialize: function() {      console.info('init:',this.collection);      this.render();      this.listenTo(this.collection, 'add', this.render);      this.listenTo(this.collection, 'reset', this.render);      this.collection.fetch();      },      // render library by rendering each book in its collection      render: function() {      var data = this.collection.toJSON();      console.log('col', this.collection );  // added      this.$el.html( this.template( {beatles: data} ));      return this;      }      });      </code></pre>      <p><strong>This is my Collection</strong></p>      <pre><code>var app = app || {};      app.BeatlesCollection = Backbone.Collection.extend({      model: app.Beatle,      url: 'http://localhost/path/to/beatles',      initialize: function() {      console.log('Init Collection');      }      });      </code></pre>      <p><strong>This is my Model</strong></p>      <pre><code>var app = app || {};      // create a model to represent a single transaction on a statement      app.Transaction = Backbone.Model.extend({});      </code></pre>      <p><strong>This is what a <code>console.log('col', this.collection );</code> in my view's render method show:</strong></p>      <pre><code>col child {length: 1, models: Array[1], _byId: Object, _listenerId: \"l2\", _events: Object…}      _byId: Object      _events: Object      _listenerId: \"l2\"      length: 1      models: Array[1]      0: child      _changing: false      _events: Object      _pending: false      _previousAttributes: Object      attributes: Object      amount: 205.99      currency: \"USD\"      date: \"2013-05-13\"      id: 13      vendor: \"Reebok Outlet\"      __proto__: Object      changed: Object      cid: \"c3\"      collection: child      id: 13      __proto__: Surrogate      length: 1      __proto__: Array[0]      __proto__: Surrogate      </code></pre>      <p><strong>My Handlebars Template looks like this:</strong></p>      <pre><code>&lt;h1&gt;Your Statement&lt;/h1&gt;      &lt;table border=\"1\"&gt;      &lt;thead&gt;      &lt;th&gt;Name&lt;/th&gt;      &lt;th&gt;Email&lt;/th&gt;      &lt;th&gt;Age&lt;/th&gt;      &lt;/thead&gt;      &lt;tbody&gt;      {{#each beatle}}      &lt;tr&gt;      &lt;td&gt;{{this.fname}} {{this.surname}}&lt;/td&gt;      &lt;td&gt;{{this.email}}&lt;/td&gt;      &lt;td&gt;{{this.age}}&lt;/td&gt;      &lt;/tr&gt;      {{/each}}      &lt;/tbody&gt;      &lt;/table&gt;      </code></pre>",
            tags: "<json><rest><backbone.js>"
            );

            // Update Parents
            foreach (var post in posts)
                if (post.ParentId.HasValue)
                    post.Parent = posts.SingleOrDefault(p => p.ID == post.ParentId.Value);

            return posts;
        }

    }
}
