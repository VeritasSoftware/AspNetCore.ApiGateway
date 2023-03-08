import fetch from 'node-fetch';
import { ApiGatewayClientSettings } from './ApiGatewayClientSettings';
import { ApiGatewayParameters } from "./ApiGatewayParameters";
import { IApiGatewayClient } from "./IApiGatewayClient";

export class ApiGatewayClient implements IApiGatewayClient {

    _settings: ApiGatewayClientSettings;
    _httpsAgent: any;

    constructor(settings: ApiGatewayClientSettings) {
        this._settings = settings;

        const https = require('https');

        var rootCas = require('ssl-root-cas').create();

        this._httpsAgent = new https.Agent({
            ca: rootCas,
            rejectUnauthorized: false,
            requestCert: true,
            agent: false            
        });
    }

    async GetAsync<TResponse>(parameters: ApiGatewayParameters): Promise<TResponse> {
        let gatewayUrl = `${this._settings.ApiGatewayBaseUrl}/api/Gateway/${parameters.Api}/${parameters.Key}?parameters=${parameters.Parameters??""}`;        
        
        const response = await fetch(gatewayUrl, {method: 'GET', agent: this._httpsAgent});
        const data = await response.json();

        return <TResponse> data;        
    }

}