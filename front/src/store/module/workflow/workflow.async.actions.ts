import { createAsyncActionGenerator, getService } from "../../common/common.actions";
import { UpdateSocketService } from "../../../core/services/sockets/update.socket.service";
import { getConversations, refreshConversation } from "../conversations/conversations.async.actions";
import { deleteConversationStore } from "../conversations/conversations.actions";

const createAsyncThunk = createAsyncActionGenerator("workflow");

export const initApp = createAsyncThunk("initApp", (_, { dispatch }) => {
	dispatch(initWebSocket());
	dispatch(getConversations());
});


export const initWebSocket = createAsyncThunk("init-websocket", async (_, { extra, dispatch }) => {

	const updateSocketService = getService(UpdateSocketService, extra);

	const socket = await updateSocketService.createSocket();

	socket.on("ConversationUpdated", id => {
		dispatch(refreshConversation(id));
	});

	socket.on("ConversationDeleted", id => {
		dispatch(deleteConversationStore(id));
	});

});