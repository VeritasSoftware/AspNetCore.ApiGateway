export class ApiGatewayHeaders {
    headers: any[] = [];

    add(key: string, value: string) : void {
        this.headers.push({key: key, value: value});
    }

    toHeaders() : any[] {
        return this.headers;
    }
}