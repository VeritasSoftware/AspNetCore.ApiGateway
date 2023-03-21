import { ApiGatewayHeaders } from "./ApiGatewayHeaders";

export class ApiGatewayParameters {
    Api?: string = "";
    Key?: string = "";
    Parameters?: string = "";
    Headers?: ApiGatewayHeaders;
}