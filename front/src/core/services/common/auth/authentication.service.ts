import { inject, injectable } from "inversify";
import { AuthenticationApiClient } from "../../../apis/authentication";
import { EventManager } from "../../../utils/event";
import { BaseService } from "../technical/base.service";
import { User } from "../../../apis/authentication/generated";
import { LocalStorageService } from "../localStorage.service";
import { openPage } from "../../../utils/web";
import { TokenService } from "./token.service";
import { DiKeysService } from "../../../di/services/di.keys.service";

@injectable()
export class AuthenticationService extends BaseService {
	constructor(
		@inject(DiKeysService.localStorage.jwt) private localStorage: LocalStorageService,
		@inject(AuthenticationApiClient) private authenticationApi: AuthenticationApiClient
	) {
		super();
	}

	public openLoginPage() {
		return openPage(`${window.config.endpoints.authentication}/login`);
	}

	public async logout() {
		await this.localStorage.remove();
		AuthenticationEvents.emit("logout");
	}

	public isValid() {
		return this.authenticationApi.jwt.verify();
	}
}

export const AuthenticationEvents = new EventManager<{
	login: (user: User) => void;
	logout: () => void;
}>();
