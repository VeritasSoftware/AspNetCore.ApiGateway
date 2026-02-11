namespace AspNetCore.ApiGateway.Minimal
{    
    public class ApiInfo
    {
        public string BaseUrl { get; set; }

        public IMediator Mediator { get; set; }
    }


    internal class LoadBalancing
    {
        public LoadBalancingType Type { get; set; } = LoadBalancingType.Random;
        public string[] BaseUrls {  get; set; }
        public int LastBaseUrlIndex { get; set; } = -1;
    }

    public enum LoadBalancingType
    {
        Random,
        RoundRobin
    }

    public class ApiOrchestrator : IApiOrchestrator
    {
        Dictionary<string, ApiInfo> apis = new Dictionary<string, ApiInfo>();

        Dictionary<string, LoadBalancing> apiLoadBalancing = new Dictionary<string, LoadBalancing>();

        private static Random _random = new Random();
        private static readonly object _syncLock = new object();

        private readonly IMediator _mediator;

        public string GatewayHubUrl { get; set; }
        public bool StartGatewayHub { get; set; }

        public ApiOrchestrator(IMediator mediator)
        {
            _mediator = mediator;
            _mediator.ApiOrchestrator = this;
        }

        public IMediator AddApi(string apiKey, params string[] baseUrls)
        {            
            var mediator = _mediator;

            apis.Add(apiKey.ToLower(), new ApiInfo() { BaseUrl = baseUrls.First(), Mediator = mediator });

            apiLoadBalancing.Add(apiKey.ToLower(), new LoadBalancing { BaseUrls = baseUrls });

            return mediator;
        }

        public IMediator AddApi(string apiKey, LoadBalancingType loadBalancingType, params string[] baseUrls)
        {
            var mediator = _mediator;

            apis.Add(apiKey.ToLower(), new ApiInfo() { BaseUrl = baseUrls.First(), Mediator = mediator });

            apiLoadBalancing.Add(apiKey.ToLower(), new LoadBalancing { Type = loadBalancingType,  BaseUrls = baseUrls });

            return mediator;
        }

        public ApiInfo GetApi(string apiKey, bool withLoadBalancing = false)
        {
            var apiInfo = apis[apiKey.ToLower()];

            if (withLoadBalancing)
            {
                //if more than 1 base url is specified
                //get the load balancing base url based on load balancing algorithm specified.

                var loadBalancing = apiLoadBalancing[apiKey.ToLower()];
                
                if (loadBalancing.BaseUrls.Count() > 1)
                {
                    apiInfo.BaseUrl = this.GetLoadBalancedUrl(loadBalancing);
                }
            }            

            return apiInfo;
        }

        public IEnumerable<Orchestration> Orchestration => new List<Orchestration>().Union(apis?.Select(x => new ApiOrchestration
        {
            Api = x.Key,
            Routes = x.Value.Mediator.Routes.Cast<RouteBase>().ToList(),
            ApiRoutes = x.Value.Mediator.Routes
        }));        

        private string GetLoadBalancedUrl(LoadBalancing loadBalancing)
        {
            if (loadBalancing.Type == LoadBalancingType.RoundRobin)
            {
                loadBalancing.LastBaseUrlIndex++;

                if (loadBalancing.LastBaseUrlIndex >= loadBalancing.BaseUrls.Length)
                {
                    loadBalancing.LastBaseUrlIndex = 0;
                }
                return loadBalancing.BaseUrls[loadBalancing.LastBaseUrlIndex];
            }
            else
            {   
                lock(_syncLock)
                {
                    var selected = _random.Next(0, loadBalancing.BaseUrls.Count());
                    return loadBalancing.BaseUrls[selected];
                }                
            }
        }
    }
}
