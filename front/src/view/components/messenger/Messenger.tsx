import React from "react";
import { Paper, Stack } from "@mui/material";
import { ConversationList } from "./list/ConversationList";
import { ConversationDetail } from "./detail/ConversationDetail";
import Divider from "@mui/material/Divider";


export function Messenger() {
	return (
		<Paper sx={{ p: 2, left: 0, right: 0, top: 0, bottom: 0, width: "80vw", height: "90%" }}>
			<Stack direction={"row"} spacing={3} justifyContent={"space-between"} width={"100%"} height={"100%"}>
				<ConversationList></ConversationList>
				<Divider orientation={"vertical"} flexItem />
				<ConversationDetail></ConversationDetail>
			</Stack>
		</Paper>
	);
}
