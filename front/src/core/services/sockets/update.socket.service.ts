import * as signalR from "@microsoft/signalr";
import { HubConnection, LogLevel } from "@microsoft/signalr";
import { Conversation } from "../../apis/backend/generated";
import { injectable } from "inversify";

interface UpdateHub extends HubConnection {
	on(event: "ConversationUpdated", callback: (id: Conversation["id"]) => void);

	on(event: "ConversationDeleted", callback: (id: Conversation["id"]) => void);

}

@injectable()
export class UpdateSocketService {
	async createSocket() {
		const connection = new signalR.HubConnectionBuilder()
			.withUrl(`${window.config.endpoints.core}/ws/update`)
			.configureLogging(LogLevel.Information)
			.withAutomaticReconnect({ nextRetryDelayInMilliseconds: () => 5000 })
			.build();

		await connection.start();
		return connection as UpdateHub;
	}
}
