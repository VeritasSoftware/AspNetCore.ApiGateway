import { ApiGatewayParameters } from "./ApiGatewayParameters";

export interface IApiGatewayClient {
    GetAsync<TResponse>(parameters: ApiGatewayParameters): Promise<TResponse>;
}