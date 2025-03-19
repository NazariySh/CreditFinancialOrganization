import { RefreshToken } from "../auth/refresh-token";
import { Address } from "./address";

export interface User {
    id: string;
    firstName: string;
    lastName: string;
    email: string;
    phoneNumber: string;
    birthDate?: Date;
    address?: Address;
    roles: string[];

    refreshToken?: RefreshToken;
}