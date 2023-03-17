import fetch from 'node-fetch';
import path from 'path';
import fs from 'fs';
import { ApiGatewayClientSettings } from './ApiGatewayClientSettings';
import { ApiGatewayParameters } from "./ApiGatewayParameters";
import { IApiGatewayClient } from "./IApiGatewayClient";
import { JsonPatchOperation } from './JsonPatch';
import { Orchestration } from './Orchestration';

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
                if (this._settings.UseCertificate) {
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
    }

    async GetAsync<TResponse>(parameters: ApiGatewayParameters): Promise<TResponse> {
        let gatewayUrl = `${this._settings.ApiGatewayBaseUrl}/api/Gateway/${parameters.Api}/${parameters.Key}?parameters=${parameters.Parameters??""}`;    
        
        var options = this._settings.IsDEVMode || this._settings.UseCertificate ? {method: 'GET', agent: this._httpsAgent} : {method: 'GET'};
        
        const response = await fetch(gatewayUrl, options);
        const res = await response.json();

        return <TResponse> res;        
    }

    async PostAsync<TPayload, TResponse>(parameters: ApiGatewayParameters, data: TPayload): Promise<TResponse> {
        let gatewayUrl = `${this._settings.ApiGatewayBaseUrl}/api/Gateway/${parameters.Api}/${parameters.Key}?parameters=${parameters.Parameters??""}`;
        
        let headers = { 'Content-Type': 'application/json' };
        let body = JSON.stringify(data);

        var options = this._settings.IsDEVMode || this._settings.UseCertificate ? {method: 'POST', body: body, headers: headers, agent: this._httpsAgent} 
                                                                                : {method: 'POST', body: body, headers: headers};
        
        const response = await fetch(gatewayUrl, options);
        const res = await response.json();

        return <TResponse> res;        
    }
    
    async PutAsync<TPayload, TResponse>(parameters: ApiGatewayParameters, data: TPayload): Promise<TResponse> {
        let gatewayUrl = `${this._settings.ApiGatewayBaseUrl}/api/Gateway/${parameters.Api}/${parameters.Key}?parameters=${parameters.Parameters??""}`;
        
        let headers = { 'Content-Type': 'application/json' };
        let body = JSON.stringify(data);

        var options = this._settings.IsDEVMode || this._settings.UseCertificate ? {method: 'PUT', body: body, headers: headers, agent: this._httpsAgent} 
                                                                                : {method: 'PUT', body: body, headers: headers};
        
        const response = await fetch(gatewayUrl, options);

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

        var options = this._settings.IsDEVMode || this._settings.UseCertificate ? {method: 'PATCH', body: body, headers: headers, agent: this._httpsAgent} 
                                                                                : {method: 'PATCH', body: body, headers: headers};
        
        const response = await fetch(gatewayUrl, options);

        if (response.ok) {
            const res = await response.json();

            return <TResponse> res;
        }
                
        return <TResponse>{};
    } 

    async DeleteAsync<TResponse>(parameters: ApiGatewayParameters): Promise<TResponse> {
        let gatewayUrl = `${this._settings.ApiGatewayBaseUrl}/api/Gateway/${parameters.Api}/${parameters.Key}?parameters=${parameters.Parameters??""}`;     
        
        var options = this._settings.IsDEVMode || this._settings.UseCertificate ? {method: 'DELETE', agent: this._httpsAgent} : {method: 'DELETE'};
        
        const response = await fetch(gatewayUrl, options);
        
        if (response.ok && response.bodyUsed) {
            const res = await response.json();

            return <TResponse> res;
        }

        return <TResponse>{};        
    }

    async GetOrchestrationAsync(parameters: ApiGatewayParameters): Promise<Orchestration[]> {
        let gatewayUrl = `${this._settings.ApiGatewayBaseUrl}/api/Gateway/orchestration?api=${parameters.Api}&key=${parameters.Key}`;   
        
        var options = this._settings.IsDEVMode || this._settings.UseCertificate ? {method: 'GET', agent: this._httpsAgent} : {method: 'GET'};
        
        const response = await fetch(gatewayUrl, options);
        const res = await response.json();

        return <Orchestration[]> res;        
    }    
}