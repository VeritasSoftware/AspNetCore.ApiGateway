import fetch from 'node-fetch';
import path from 'path';
import fs from 'fs';
import { ApiGatewayClientSettings } from './ApiGatewayClientSettings';
import { ApiGatewayParameters } from "./ApiGatewayParameters";
import { IApiGatewayClient } from "./IApiGatewayClient";
import { JsonPatchOperation } from './JsonPatch';

export class ApiGatewayClient implements IApiGatewayClient {

    _settings: ApiGatewayClientSettings;
    _httpsAgent: any;

    constructor(settings: ApiGatewayClientSettings) {
        this._settings = settings;

        if (this._settings.UseHttps) {
            const https = require('https');

            if (this._settings.IsDEVMode) {
                this._httpsAgent = new https.Agent({
                    rejectUnauthorized: false,
                    requestCert: true,
                    agent: false            
                });
            }
            else {
                const options = {
                    cert: fs.readFileSync(
                      path.resolve(__dirname, this._settings.HttpsSettings?.PfxPath!),
                      `utf-8`,
                    ),
                    key: fs.readFileSync(
                      path.resolve(__dirname, this._settings.HttpsSettings?.PrivateKeyPath!),
                      'utf-8',
                    ),
                    passphrase:
                        this._settings.HttpsSettings?.Passphrase!,
                
                    rejectUnauthorized: true,
                
                    keepAlive: true,
                  };
                  this._httpsAgent = new https.Agent(options);
            }            
        }      
    }

    async GetAsync<TResponse>(parameters: ApiGatewayParameters): Promise<TResponse> {
        let gatewayUrl = `${this._settings.ApiGatewayBaseUrl}/api/Gateway/${parameters.Api}/${parameters.Key}?parameters=${parameters.Parameters??""}`;        
        
        const response = await fetch(gatewayUrl, {method: 'GET', agent: this._httpsAgent});
        const res = await response.json();

        return <TResponse> res;        
    }

    async PostAsync<TPayload, TResponse>(parameters: ApiGatewayParameters, data: TPayload): Promise<TResponse> {
        let gatewayUrl = `${this._settings.ApiGatewayBaseUrl}/api/Gateway/${parameters.Api}/${parameters.Key}?parameters=${parameters.Parameters??""}`;
        
        let headers = { 'Content-Type': 'application/json' };
        let body = JSON.stringify(data);
        
        const response = await fetch(gatewayUrl, {method: 'POST', body: body, headers: headers, agent: this._httpsAgent});
        const res = await response.json();

        return <TResponse> res;        
    }
    
    async PutAsync<TPayload, TResponse>(parameters: ApiGatewayParameters, data: TPayload): Promise<TResponse> {
        let gatewayUrl = `${this._settings.ApiGatewayBaseUrl}/api/Gateway/${parameters.Api}/${parameters.Key}?parameters=${parameters.Parameters??""}`;
        
        let headers = { 'Content-Type': 'application/json' };
        let body = JSON.stringify(data);
        
        const response = await fetch(gatewayUrl, {method: 'PUT', body: body, headers: headers, agent: this._httpsAgent});

        if (response.ok && response.bodyUsed) {
            const res = await response.json();

            return <TResponse> res;
        }
                
        return <TResponse>{};
    }    
    
    async PatchAsync<TResponse>(parameters: ApiGatewayParameters, data: JsonPatchOperation[]): Promise<TResponse> {
        let gatewayUrl = `${this._settings.ApiGatewayBaseUrl}/api/Gateway/${parameters.Api}/${parameters.Key}?parameters=${parameters.Parameters??""}`;
        
        let headers = { 'Content-Type': 'application/json-patch+json' };
        let body = JSON.stringify(data);
        
        const response = await fetch(gatewayUrl, {method: 'PATCH', body: body, headers: headers, agent: this._httpsAgent});

        if (response.ok) {
            const res = await response.json();

            return <TResponse> res;
        }
                
        return <TResponse>{};
    } 

    async DeleteAsync<TResponse>(parameters: ApiGatewayParameters): Promise<TResponse> {
        let gatewayUrl = `${this._settings.ApiGatewayBaseUrl}/api/Gateway/${parameters.Api}/${parameters.Key}?parameters=${parameters.Parameters??""}`;        
        
        const response = await fetch(gatewayUrl, {method: 'DELETE', agent: this._httpsAgent});
        
        if (response.ok && response.bodyUsed) {
            const res = await response.json();

            return <TResponse> res;
        }

        return <TResponse>{};        
    }
}