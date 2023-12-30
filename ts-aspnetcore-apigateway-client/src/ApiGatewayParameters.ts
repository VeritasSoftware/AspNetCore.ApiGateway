import { ApiGatewayHeaders } from "./ApiGatewayHeaders";

export class ApiGatewayParameters {
    ApiKey?: string = "";
    RouteKey?: string = "";
    Parameters?: string = "";
    Headers?: ApiGatewayHeaders;
}