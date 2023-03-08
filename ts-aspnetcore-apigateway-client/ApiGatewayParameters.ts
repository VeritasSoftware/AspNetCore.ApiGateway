import { Dictionary, IEnumerable } from "ts-generic-collections-linq";

export class ApiGatewayParameters {
    Api?: string;
    Key?: string;
    Parameters?: string;
    HeaderLists?: Dictionary<string, IEnumerable<string>>;
    Headers?: Dictionary<string, string>;
}