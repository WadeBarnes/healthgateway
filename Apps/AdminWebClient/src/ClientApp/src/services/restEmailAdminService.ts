import { injectable } from "inversify";
import { Dictionary } from "vue-router/types/router";

import Email from "@/models/email";
import RequestResult from "@/models/requestResult";
import { IEmailAdminService, IHttpDelegate } from "@/services/interfaces";
import RequestResultUtil from "@/utility/requestResultUtil";

@injectable()
export class RestEmailAdminService implements IEmailAdminService {
    private readonly BASE_URI: string = "v1/api/EmailAdmin";
    private http!: IHttpDelegate;

    public initialize(http: IHttpDelegate): void {
        this.http = http;
    }

    public getEmails(): Promise<Email[]> {
        return new Promise((resolve, reject) => {
            this.http
                .get<RequestResult<Email[]>>(`${this.BASE_URI}`)
                .then((requestResult) => {
                    console.debug(`getEmails ${requestResult}`);
                    return RequestResultUtil.handleResult(
                        requestResult,
                        resolve,
                        reject
                    );
                })
                .catch((err) => {
                    console.log(err);
                    return reject(err);
                });
        });
    }

    public resendEmails(emailIds: string[]): Promise<string[]> {
        return new Promise((resolve, reject) => {
            const headers: Dictionary<string> = {};
            headers["Content-Type"] = "application/json; charset=utf-8";
            this.http
                .post<RequestResult<string[]>>(
                    `${this.BASE_URI}`,
                    JSON.stringify(emailIds),
                    headers
                )
                .then((requestResult) => {
                    console.debug(`resendEmails ${requestResult}`);
                    return RequestResultUtil.handleResult(
                        requestResult,
                        resolve,
                        reject
                    );
                })
                .catch((err) => {
                    console.log(err);
                    return reject(err);
                });
        });
    }
}
