import { ResultType } from "@/constants/resulttype";
import Encounter from "@/models/encounter";
import RequestResult, { ResultError } from "@/models/requestResult";
import { LoadStatus } from "@/models/storeOperations";
import { SERVICE_IDENTIFIER } from "@/plugins/inversify";
import container from "@/plugins/inversify.container";
import { IEncounterService, ILogger } from "@/services/interfaces";

import { EncounterActions } from "./types";

export const actions: EncounterActions = {
    retrieve(
        context,
        params: { hdid: string }
    ): Promise<RequestResult<Encounter[]>> {
        const logger: ILogger = container.get(SERVICE_IDENTIFIER.Logger);
        const encounterService: IEncounterService = container.get<IEncounterService>(
            SERVICE_IDENTIFIER.EncounterService
        );

        return new Promise((resolve, reject) => {
            const patientEncounters: Encounter[] =
                context.getters.patientEncounters;
            if (context.state.status === LoadStatus.LOADED) {
                logger.debug(`Encounters found stored, not quering!`);
                resolve({
                    pageIndex: 0,
                    pageSize: 0,
                    resourcePayload: patientEncounters,
                    resultStatus: ResultType.Success,
                    totalResultCount: patientEncounters.length,
                });
            } else {
                logger.debug(`Retrieving Patient Encounters`);
                context.commit("setRequested");
                encounterService
                    .getPatientEncounters(params.hdid)
                    .then((result) => {
                        if (result.resultStatus === ResultType.Error) {
                            context.dispatch("handleError", result.resultError);
                            reject(result.resultError);
                        } else {
                            context.commit(
                                "setPatientEncounters",
                                result.resourcePayload
                            );
                            resolve(result);
                        }
                    })
                    .catch((error) => {
                        context.dispatch("handleError", error);
                        reject(error);
                    });
            }
        });
    },
    handleError(context, error: ResultError) {
        const logger: ILogger = container.get(SERVICE_IDENTIFIER.Logger);

        logger.error(`ERROR: ${JSON.stringify(error)}`);
        context.commit("encounterError", error);
        context.dispatch(
            "errorBanner/addResultError",
            { message: "Fetch Encounter Error", error },
            { root: true }
        );
    },
};
