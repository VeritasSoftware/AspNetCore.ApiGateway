import { ApiGatewayParameters } from "./ApiGatewayParameters";
import { JsonPatchOperation } from "./JsonPatch";

export interface IApiGatewayClient {
    GetAsync<TResponse>(parameters: ApiGatewayParameters): Promise<TResponse>;
    PostAsync<TPayload, TResponse>(parameters: ApiGatewayParameters, data: TPayload): Promise<TResponse>;
    PutAsync<TPayload, TResponse>(parameters: ApiGatewayParameters, data: TPayload): Promise<TResponse>;
    PatchAsync<TResponse>(parameters: ApiGatewayParameters, data: JsonPatchOperation[]): Promise<TResponse>;
}