import { Conversation, Message } from "../../../core/apis/backend/generated";
import { createAsyncActionGenerator } from "../../common/common.actions";
import { ConversationService } from "../../../core/services/conversation.service";

const createAsyncThunk = createAsyncActionGenerator("todo");

export const getConversations = createAsyncThunk("getTodo", async (search: string | undefined, { extra }) => {
	const { container } = extra;
	const service = container.get(ConversationService);

	return service.search(search);
});

export const deleteConversation = createAsyncThunk("delete", (id: Conversation["id"], { extra }) => {
	const { container } = extra;
	const service = container.get(ConversationService);

	return service.delete(id);
});

type RenameConversationParams = {
	id: Conversation["id"],
	title: Conversation["title"]
};
export const renameConversation = createAsyncThunk("rename", ({ id, title }: RenameConversationParams, { extra }) => {
	const { container } = extra;
	const service = container.get(ConversationService);

	return service.rename(id, title);
});


type AddMessageToConversationParams = {
	author: Message["author"]["name"],
	content: Message["content"]
	id: Conversation["id"]
};
export const addMessageToConversation = createAsyncThunk("add-message", (args: AddMessageToConversationParams, { extra }) => {

	const service = extra.container.get(ConversationService);

	return service.addMessage(args.id, args.content, args.author);
});


export const refreshConversation = createAsyncThunk("refresh", (id: Conversation["id"], {extra}) => {
	const service = extra.container.get(ConversationService);

	return service.get(id);
});