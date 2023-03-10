import { ApiGatewayParameters } from "./ApiGatewayParameters";

export interface IApiGatewayClient {
    GetAsync<TResponse>(parameters: ApiGatewayParameters): Promise<TResponse>;
    PostAsync<TPayload, TResponse>(parameters: ApiGatewayParameters, data: TPayload): Promise<TResponse>;
}