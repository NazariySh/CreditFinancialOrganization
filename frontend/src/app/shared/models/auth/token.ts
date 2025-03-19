import { RefreshToken } from "./refresh-token";

export interface Token {
    accessToken: string;
    refreshToken: RefreshToken;
}