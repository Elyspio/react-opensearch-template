import React, { useEffect, useMemo, useState } from "react";
import { Box, Stack, TextField, Typography } from "@mui/material";
import { useAppDispatch, useAppSelector } from "../../../../store";
import { addMessageToConversation } from "../../../../store/module/conversations/conversations.async.actions";
import IconButton from "@mui/material/IconButton";
import { Send } from "@mui/icons-material";


export function ConversationDetail() {

	const dispatch = useAppDispatch();
	const { all, selected } = useAppSelector(s => s.conversations);

	const conversation = useMemo(() => all[selected!], [all, selected]);

	const [author, setAuthor] = useState("");

	const [content, setContent] = useState("");

	const send = () => dispatch(addMessageToConversation({ id: conversation.id, author, content: content }));


	useEffect(() => {
		if (conversation) setAuthor(conversation.members[0].name);
	}, [conversation?.id]);

	const handleUpdate = (type: "author" | "content") => (e: React.ChangeEvent<HTMLInputElement>) => {
		const handler = type === "author" ? setAuthor : setContent;
		handler(e.target.value);
	};


	const members = (conversation?.members ?? []).map(member => member.name).join(", ");

	if (!conversation) return <Stack width={"100%"}></Stack>;


	return (
		<Stack width={"100%"} spacing={4}>
			<Typography variant={"h5"}> {conversation.title}</Typography>
			<Typography>Members: {members}</Typography>
			<Stack direction={"row"} spacing={2} alignItems={"center"}>
				<TextField onChange={handleUpdate("author")} label={"Author"} value={author}></TextField>
				<TextField fullWidth onChange={handleUpdate("content")} label={"Content"} value={content}></TextField>
				<Box>
					<IconButton color={"warning"} onClick={send}><Send /></IconButton>
				</Box>
			</Stack>


			<pre>
				{JSON.stringify(conversation, null, 4)}
			</pre>
		</Stack>
	);
}
