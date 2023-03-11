export class Orchestration {
    Api?: string;
    OrchestrationType?: OrchestationType;
}

export enum OrchestationType {
    Api,
    Hub,
    EventSource
}

export class ApiOrchestration extends Orchestration {
    ApiRoutes?: Route[];
}

export class HubOrchestration extends Orchestration {
    HubRoutes?: HubRoute[];
}

export class EventSourceOrchestration extends Orchestration {
    EventSourceRoutes?: EventSourceRoute[]
}

export class RouteBase {
    Key?: string;
}

export class Route extends RouteBase {
    Verb?: string;
    DownstreamPath?: string;
}

export class HubRoute extends RouteBase {
    InvokeMethod?: string;
    ReceiveMethod?: string;
    ReceiveGroup?: string;
    BroadcastType?: string;
    ReceiveParameterTypes?: string[];
}

export class EventSourceRoute extends RouteBase {
    Type?: string;
    ReceiveMethod?: string;
    OperationType?: string;
    StreamName?: string;
    GroupName?: string;
}