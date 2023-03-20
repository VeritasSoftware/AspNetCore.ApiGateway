export class ApiGatewayClientSettings {
    ApiGatewayBaseUrl?: string;
    IsDEVMode?: boolean;
    UseCertificate?: boolean;
    CertificateSettings?: CertificateSettings;
}

export class CertificateSettings {
    PfxPath?: string; //Eg './path/to/public-cert.pem'
    PrivateKeyPath?: string; //Eg './path/to/private-key.key'
    Passphrase?: string;
}