import { createActionGenerator } from "../../common/common.actions";
import { Conversation } from "../../../core/apis/backend/generated";
import { ConversationService } from "../../../core/services/conversation.service";

const createAction = createActionGenerator("conversations");


export const setSelected = createAction<Conversation["id"] | undefined>("setConversation");

export const deleteConversationStore = createAction<Conversation["id"]>("")