import { Dictionary } from "ts-generic-collections-linq";

export class ApiGatewayParameters {
    Api?: string = "";
    Key?: string = "";
    Parameters?: string = "";
    Headers?: Dictionary<string, string>;
}