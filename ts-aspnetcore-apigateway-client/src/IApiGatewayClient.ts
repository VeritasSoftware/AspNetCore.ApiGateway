import { ApiGatewayParameters } from "./ApiGatewayParameters";
import { JsonPatchOperation } from "./JsonPatch";
import { Orchestration } from "./Orchestration";

export interface IApiGatewayClient {
    GetAsync<TResponse>(parameters: ApiGatewayParameters): Promise<TResponse>;
    PostAsync<TPayload, TResponse>(parameters: ApiGatewayParameters, data: TPayload): Promise<TResponse | null>;
    PutAsync<TPayload, TResponse>(parameters: ApiGatewayParameters, data: TPayload): Promise<TResponse | null>;
    PatchAsync<TResponse>(parameters: ApiGatewayParameters, data: JsonPatchOperation[]): Promise<TResponse | null>;
    DeleteAsync<TResponse>(parameters: ApiGatewayParameters): Promise<TResponse | null>;
    GetOrchestrationAsync(parameters: ApiGatewayParameters): Promise<Orchestration[]>;
}