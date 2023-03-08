import { createSlice } from "@reduxjs/toolkit";
import { Conversation } from "../../../core/apis/backend/generated";
import { deleteConversationStore, setSelected } from "./conversations.actions";
import { getConversations, refreshConversation } from "./conversations.async.actions";

export type ConversationState = {
	all: Record<string, Conversation>
	selected?: string
};

const initialState: ConversationState = {
	all: {},
};

const slice = createSlice({
	name: "todo",
	initialState,
	reducers: {},
	extraReducers: (builder) => {

		builder.addCase(setSelected, (state, action) => {
			state.selected = action.payload;
		});

		builder.addCase(deleteConversationStore, (state, action) => {
			if (state.selected === action.payload) state.selected = undefined;
			delete state.all[action.payload];
		});

		builder.addCase(getConversations.fulfilled, (state, action) => {

			state.all = {};

			action.payload.forEach(conv => {
				state.all[conv.id] = conv;
			});
		});

		builder.addCase(refreshConversation.fulfilled, (state, action) => {
			state.all[action.payload.id] = action.payload;
		});
	},
});

export const conversationsReducer = slice.reducer;
