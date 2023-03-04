import { createAsyncThunk as _createAsyncThunk } from "@reduxjs/toolkit";
import { ExtraArgument } from "../index";
import { ActionCreatorWithPayload, AsyncThunkPayloadCreator, createAction as _createAction } from "@reduxjs/toolkit";
import { AsyncThunkFulfilledActionCreator, AsyncThunkPendingActionCreator, AsyncThunkRejectedActionCreator } from "@reduxjs/toolkit/dist/createAsyncThunk";

type Constructor<T> = new (...args: any[]) => T;

export function getService<T>(service: Constructor<T>, extra): T {
	const { container } = extra as ExtraArgument;
	return container.get(service);
}

type ActionCreator = AsyncThunkPendingActionCreator<any, any> | AsyncThunkRejectedActionCreator<any, any> | AsyncThunkFulfilledActionCreator<any, any>;

export function throwIfRejected(action: ReturnType<ActionCreator>) {
	if (action.meta.requestStatus === "rejected") throw new Error((action as any).error.message);
}

export function createReplaceAction<T>(creator: <T>(module: string) => any): ActionCreatorWithPayload<T, string> {
	return creator("replace");
}

export function createAsyncActionGenerator(prefix: string) {
	return function createAsyncThunk<Returned, ThunkArg = void>(suffix: string, payloadCreator: AsyncThunkPayloadCreator<Returned, ThunkArg, { extra: ExtraArgument }>) {
		return _createAsyncThunk<Returned, ThunkArg>(`${prefix}/${suffix}`, payloadCreator);
	};
}

export function createActionGenerator(prefix: string) {
	return <T = void>(suffix: string) => _createAction<T>(`${prefix}/${suffix}`);
}
