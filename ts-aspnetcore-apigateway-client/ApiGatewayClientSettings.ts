export class ApiGatewayClientSettings {
    ApiGatewayBaseUrl?: string;
    IsDEVMode?: boolean;
    UseHttps?: boolean;
    HttpsSettings?: HttpsSettings;
}

export class HttpsSettings {
    PfxPath?: string; //Eg './path/to/public-cert.pem'
    PrivateKeyPath?: string; //Eg './path/to/private-key.key'
    Passphrase?: string;
}