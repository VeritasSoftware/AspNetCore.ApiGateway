export class JsonPatchOperation {
    op?: Operation;
    path?: string;
    from?: string;
    value?: any;
}

export enum Operation
{
    Add = "Add",
    Remove = "Remove",
    Replace = "Replace",
    Move = "Move",
    Copy = "Copy",
    Test = "Test",
    Invalid = "Invalid"
}