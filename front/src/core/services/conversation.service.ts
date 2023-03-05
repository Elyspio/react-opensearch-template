import { inject, injectable } from "inversify";
import { BackendApi } from "../apis/backend";
import { BaseService } from "./common/technical/base.service";


@injectable()
export class ConversationService extends BaseService {


	@inject(BackendApi)
	private backendApiClient!: BackendApi;


	addMessage(id: string, content: string, author: string): Promise<void> {
		return this.backendApiClient.conversations.addMessage(id, {
			content,
			author,
		});
	}


	get(id: string) {
		return this.backendApiClient.conversations.getById(id);
	}


	create(title: string, members: string[]) {
		return this.backendApiClient.conversations.create({
			title,
			members,
		});
	}

	delete(id: string) {
		return this.backendApiClient.conversations.delete(id);
	}

	rename(id: string, title: string): Promise<void> {
		return this.backendApiClient.conversations.rename(id, title);
	}


	public search(content?: string) {
		return this.backendApiClient.conversations.search(content);
	}


}
