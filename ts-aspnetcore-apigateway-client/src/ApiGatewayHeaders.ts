import { Dictionary } from "ts-generic-collections-linq";

export class ApiGatewayHeaders {
    headers: Dictionary<string, string>;

    constructor() {
        this.headers = new Dictionary<string, string>();
    }

    add(key: string, value: string) : void {
        this.headers.add(key, value);
    }

    toHeaders() : Dictionary<string, string> {
        return this.headers;
    }
}