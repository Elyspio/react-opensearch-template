import { ExtraArgument } from "../../index";
import { TodoService } from "../../../core/services/todo.service";
import { TodoState } from "./todo.reducer";
import { Todo } from "../../../core/apis/backend/generated";
import { createAsyncActionGenerator } from "../../common/common.actions";

const createAsyncThunk = createAsyncActionGenerator("todo");

export const getTodos = createAsyncThunk("getTodo", async (mode: keyof TodoState["todos"], { extra, signal }) => {
	const { container } = extra as ExtraArgument;
	const service = container.get(TodoService);
	const fn = mode === "user" ? service.user.get : service.common.get;

	return await fn();
});

type AddTodoProps = { mode: keyof TodoState["todos"]; label: string };
export const addTodo = createAsyncThunk("addTodo", async ({ mode, label }: AddTodoProps, { extra }) => {
	const { container } = extra as ExtraArgument;
	const service = container.get(TodoService);
	const fn = mode === "user" ? service.user.add : service.common.add;
	return await fn(label);
});

type DeleteTodoProps = { mode: keyof TodoState["todos"]; id: Todo["id"] };
export const deleteTodo = createAsyncThunk("deleteTodo", async ({ mode, id }: DeleteTodoProps, { extra, signal, requestId }) => {
	const { container } = extra as ExtraArgument;
	const service = container.get(TodoService);
	const fn = mode === "user" ? service.user.remove : service.common.remove;
	return await fn(id);
});

export const checkTodo = createAsyncThunk("checkTodo", async ({ mode, id }: DeleteTodoProps, { extra }) => {
	const { container } = extra as ExtraArgument;
	const service = container.get(TodoService);
	const fn = mode === "user" ? service.user.check : service.common.check;
	return await fn(id);
});
