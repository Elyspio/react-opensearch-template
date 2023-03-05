import React, { useMemo } from "react";
import { Button, Stack } from "@mui/material";
import { DataGrid, GridColDef, GridRowsProp } from "@mui/x-data-grid";
import { useAppDispatch, useAppSelector } from "../../../../store";
import { User } from "../../../../core/apis/backend/generated";
import { bindActionCreators } from "redux";
import { deleteConversation } from "../../../../store/module/conversations/conversations.async.actions";
import { setSelected } from "../../../../store/module/conversations/conversations.actions";

export function ConversationList() {

	const { all: conversations } = useAppSelector(s => s.conversations);


	const dispatch = useAppDispatch();

	const actions = useMemo(() => bindActionCreators({ deleteConversation, setSelected }, dispatch), [dispatch]);

	const rows: GridRowsProp = useMemo(() => Object.values(conversations), [conversations]);


	const columns: GridColDef[] = [
		{ field: "title", headerName: "Title", width: 150 },
		{
			field: "members",
			headerName: "Members",
			minWidth: 300,
			renderCell: params => (params.row.members as User[]).map((m) => m.name).join(", "),
		},
		{
			field: "id", renderCell: params => <Stack>
				<Button onClick={(e) => {
					e.stopPropagation();
					actions.deleteConversation(params.row.id);
				}}>X</Button>
			</Stack>,
		},
	];


	return (
		<Stack height={"100%"} width={"100%"}>
			<DataGrid rows={rows} columns={columns} onRowClick={params => actions.setSelected(params.row.id)} />
		</Stack>
	);
}


