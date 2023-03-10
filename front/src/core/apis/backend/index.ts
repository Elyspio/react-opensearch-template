import { inject, injectable } from "inversify";
import { ConversationClient } from "./generated";
import { TokenService } from "../../services/common/auth/token.service";
import axios from "axios";

const fetch: (url: RequestInfo, init?: RequestInit) => Promise<Response> = (url, init) => {
	return window.fetch(url, {
		...init,
		credentials: "include",
	});
};

@injectable()
export class BackendApi {
	public conversations: ConversationClient;

	constructor(@inject(TokenService) tokenService: TokenService) {
		const instance = axios.create({ withCredentials: true, transformResponse: [] });

		instance.interceptors.request.use((value) => {
			value.headers!["Authorization"] = `Bearer ${tokenService.getToken()}`;
			return value;
		});

		this.conversations = new ConversationClient(window.config.endpoints.core, instance);
	}
}
