import fetch from 'node-fetch';
import { Headers } from 'node-fetch';
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

        var baseUrl = this._settings.ApiGatewayBaseUrl;
        if (baseUrl) {
            if (baseUrl.charAt(baseUrl.length - 1) === '/') {
                baseUrl = baseUrl.substring(0, baseUrl.length - 1);
                this._settings.ApiGatewayBaseUrl = baseUrl;
            }
        }                    

        if (this._settings.IsDEVMode) {
            const https = require('https');

            this._httpsAgent = new https.Agent({
                rejectUnauthorized: false,
                requestCert: true,
                agent: false            
            });
        }
        else {                        
            if (this._settings.UseCertificate && this._settings.CertificateSettings) {
                const fs = require('fs');
                const path = require('path');

                const options = {
                    cert: fs.readFileSync(
                      path.resolve(__dirname, this._settings.CertificateSettings.PfxPath!),
                      `utf-8`,
                    ),
                    key: fs.readFileSync(
                      path.resolve(__dirname, this._settings.CertificateSettings.PrivateKeyPath!),
                      'utf-8',
                    ),
                    passphrase:
                        this._settings.CertificateSettings.Passphrase!,
                
                    rejectUnauthorized: true,
                
                    keepAlive: true,
                  };
                  
                  const https = require('https');

                  this._httpsAgent = new https.Agent(options);
            }                
        }      
    }

    async GetAsync<TResponse>(parameters: ApiGatewayParameters): Promise<TResponse> {
        let gatewayUrl = `${this._settings.ApiGatewayBaseUrl}/api/Gateway/${parameters.ApiKey}/${parameters.RouteKey}?parameters=${parameters.Parameters??""}`;
        
        let headers = new Headers();
        if (parameters.Headers)
            headers = this.getHeaders(parameters.Headers.toHeaders());
        
        var options = this._settings.IsDEVMode || this._settings.UseCertificate ? {method: 'GET', agent: this._httpsAgent, headers: headers} 
                                                                                : {method: 'GET', headers: headers};                                                                    
        
        const response = await fetch(gatewayUrl, options);
        const res = await response.json();

        return <TResponse> res;        
    }

    async PostAsync<TPayload, TResponse>(parameters: ApiGatewayParameters, data: TPayload): Promise<TResponse | null> {
        let gatewayUrl = `${this._settings.ApiGatewayBaseUrl}/api/Gateway/${parameters.ApiKey}/${parameters.RouteKey}?parameters=${parameters.Parameters??""}`;
        
        let headers = new Headers();
        if (parameters.Headers)
            headers = this.getHeaders(parameters.Headers.toHeaders());        
        headers.append("Content-Type", "application/json");

        let body = JSON.stringify(data);

        var options = this._settings.IsDEVMode || this._settings.UseCertificate ? {method: 'POST', body: body, headers: headers, agent: this._httpsAgent} 
                                                                                : {method: 'POST', body: body, headers: headers};
        
        const response = await fetch(gatewayUrl, options);

        if (response.ok) {
            const res = await response.json();

            return <TResponse> res;
        }

        return null;        
    }
    
    async PutAsync<TPayload, TResponse>(parameters: ApiGatewayParameters, data: TPayload): Promise<TResponse | null> {
        let gatewayUrl = `${this._settings.ApiGatewayBaseUrl}/api/Gateway/${parameters.ApiKey}/${parameters.RouteKey}?parameters=${parameters.Parameters??""}`;
        
        let headers = new Headers();
        if (parameters.Headers)
            headers = this.getHeaders(parameters.Headers.toHeaders());        
        headers.append("Content-Type", "application/json");

        let body = JSON.stringify(data);

        var options = this._settings.IsDEVMode || this._settings.UseCertificate ? {method: 'PUT', body: body, headers: headers, agent: this._httpsAgent} 
                                                                                : {method: 'PUT', body: body, headers: headers};
        
        const response = await fetch(gatewayUrl, options);

        if (response.ok && response.bodyUsed) {
            const res = await response.json();

            return <TResponse> res;
        }
                
        return null;
    }    
    
    async PatchAsync<TResponse>(parameters: ApiGatewayParameters, data: JsonPatchOperation[]): Promise<TResponse | null> {
        let gatewayUrl = `${this._settings.ApiGatewayBaseUrl}/api/Gateway/${parameters.ApiKey}/${parameters.RouteKey}?parameters=${parameters.Parameters??""}`;
        
        let headers = new Headers();
        if (parameters.Headers)
            headers = this.getHeaders(parameters.Headers.toHeaders());        
        headers.append("Content-Type", "application/json-patch+json");

        let body = JSON.stringify(data);

        var options = this._settings.IsDEVMode || this._settings.UseCertificate ? {method: 'PATCH', body: body, headers: headers, agent: this._httpsAgent} 
                                                                                : {method: 'PATCH', body: body, headers: headers};
        
        const response = await fetch(gatewayUrl, options);

        if (response.ok) {
            const res = await response.json();

            return <TResponse> res;
        }
                
        return null;
    } 

    async DeleteAsync<TResponse>(parameters: ApiGatewayParameters): Promise<TResponse | null> {
        let gatewayUrl = `${this._settings.ApiGatewayBaseUrl}/api/Gateway/${parameters.ApiKey}/${parameters.RouteKey}?parameters=${parameters.Parameters??""}`;
        
        let headers = new Headers();
        if (parameters.Headers)
            headers = this.getHeaders(parameters.Headers.toHeaders());

        var options = this._settings.IsDEVMode || this._settings.UseCertificate ? {method: 'DELETE', agent: this._httpsAgent, headers: headers} 
                                                                                : {method: 'DELETE', headers: headers};
        
        const response = await fetch(gatewayUrl, options);
        
        if (response.ok && response.bodyUsed) {
            const res = await response.json();

            return <TResponse> res;
        }

        return null;        
    }

    async GetOrchestrationAsync(parameters: ApiGatewayParameters): Promise<Orchestration[]> {
        let gatewayUrl = `${this._settings.ApiGatewayBaseUrl}/api/Gateway/orchestration?api=${parameters.ApiKey}&key=${parameters.RouteKey}`;
        
        let headers = new Headers();
        if (parameters.Headers)
            headers = this.getHeaders(parameters.Headers.toHeaders());        
        
        var options = this._settings.IsDEVMode || this._settings.UseCertificate ? {method: 'GET', agent: this._httpsAgent, headers: headers} 
                                                                                : {method: 'GET', headers: headers};
        
        const response = await fetch(gatewayUrl, options);
        const res = await response.json();

        return <Orchestration[]> res;        
    }
    
    getHeaders(requestHeaders: any[]) : Headers {
        var headers = new Headers();
        if (requestHeaders) {
            requestHeaders.forEach(header => headers.append(header.key, header.value));
        }
        return headers;
    }    
}